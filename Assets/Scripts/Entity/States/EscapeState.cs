using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class EscapeState : MonsterStateBase
{
    Transform escapetarget;
    Transform realtarget;
    protected override void OnInitialize()
	{
        escapetarget = transform.root.GetChild(3);
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.agent.isStopped = false;
        monster.agent.speed = 10;
        realtarget = monster.target;
        monster.target = escapetarget;
    }
    protected override void OnFixedUpdate()
    {
        Vector3 v =  new Vector3(transform.position.x - realtarget.position.x, 0, transform.position.z - realtarget.position.z);
        escapetarget.position = transform.position + Vector3.Normalize(v) * 2;
    }
    protected override void OnExitState()
    {
        monster.agent.speed = 7.5f;
        monster.GetComponent<MonsterBatStateMachine>().currentRelic = -1;
    }

    protected override void OnEnterStateRender() { }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }

    protected override bool CanExitState(StateBehaviour nextState)
    {
        return Vector3.SqrMagnitude(realtarget.position - monster.transform.position) > monster.recogDistance * monster.recogDistance || Machine.StateTime >= 25;
    }
}
