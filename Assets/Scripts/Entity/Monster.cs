using UnityEngine;
using New_Neo_LT.Scripts;
using System.Xml.Serialization;
using Fusion;
using UnityEngine.AI;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using Fusion.Addons.KCC;
using System.Threading;

public enum EMonseterState
{
    None = -1,
    Spawn,
    Idle,
    Patrol,
    Chase,
    Attack,
    Return,
    Dead,
    Special,
    Count
}
public class Monster : NetworkBehaviour
{
    public EMonseterState monsterState { get; set; } = EMonseterState.None;
    public Animator animator;
    public NavMeshAgent agent;
    public Transform target = null;
    public AttackColider attackColider;
    public Vector3 spawnPosition { get; set; }
    public float recogDistance = 20;
    public float chaseDistance = 20;
    public float attackDistance = 5;

    public override void Spawned()
    {
        base.Spawned();
        agent.enabled = Object.HasStateAuthority;
        spawnPosition = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, recogDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        agent.SetDestination(target != null ? target.position : spawnPosition);
    }

    public bool CheckPlayersInRange()
    {
        target = null;

        if (PlayerRegistry.GetAllPlayers().Length <= 0)
            return false;

        float beforePlayerDistance = 0;
        float currentPlayerDistance = 0;

        foreach (PlayerCharacter player in PlayerRegistry.GetAllPlayers())
        {
            currentPlayerDistance = Vector3.SqrMagnitude(player.transform.position - transform.position);

            if (currentPlayerDistance > recogDistance * recogDistance)
                continue;

            if (beforePlayerDistance >= currentPlayerDistance)
                continue;

            beforePlayerDistance = currentPlayerDistance;
            target = player.transform;

            //if(Runner.GetPhysicsScene().Raycast(transform.position + Vector3.up, Vector3.Normalize(player.transform.position - transform.position), out RaycastHit hit, recogDistance))
            //{
            //    if (hit.collider.GetComponent<PlayerCharacter>() == player)
            //        target = player.transform;
            //}
        }

        if (target == null || !target.TryGetComponent(out PlayerCharacter c))
            return false;

        return true; ;
    }

    public bool CanAttackPlayer()
    {
        if(target == null) return false;

        if (Vector3.SqrMagnitude(target.position - transform.position) >= attackDistance * attackDistance)
            return false;

        return true;
    }
}
