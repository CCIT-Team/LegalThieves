using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using UnityEngine.AI;
using Fusion.Addons.Physics;

public class ChaseState : MonsterStateBase
{
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState(){}
    protected override void OnFixedUpdate()
    {

    }
    protected override void OnExitState() { }

    protected override void OnEnterStateRender()
    {
        monster.animator.SetTrigger("Chase");
        monster.agent.isStopped = false;
    }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }
}
