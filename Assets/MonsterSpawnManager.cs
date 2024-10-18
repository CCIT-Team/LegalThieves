using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 
public class MonsterSpawnManager : NetworkBehaviour
{
    public GameObject monsterPrefab;
    public List<Transform> worldPatrolPoints;
    public void PatrollSet()
    {
        worldPatrolPoints = GameObject.FindWithTag("MobPatrollParent").GetComponentsInChildren<Transform>().ToList();
     
    }
    public void SpawnMonster()
    {
        if (Runner.IsServer) // ���������� ���� ����
        {
            // �������� ���͸� �����ϰ� ��� Ŭ���̾�Ʈ�� ����ȭ
            NetworkObject monster = Runner.Spawn(monsterPrefab, worldPatrolPoints[Random.Range(1, worldPatrolPoints.Count-1)].position, Quaternion.identity);
            monster.GetComponent<Monster>().patrolPoints = worldPatrolPoints;
        }
    }
    
}
