using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class IdleState : MonsterStateBase
{
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.agent.isStopped = true;
        Debug.Log("Enter Idle");
    }

    protected override void OnEnterStateRender()
    {
        if(!monster.animator.GetAnimatorTransitionInfo(0).IsName("Idle"))
            monster.animator.SetTrigger("Idle");
    }

    protected override void OnFixedUpdate()
    {
        
    }
    protected override void OnRender() { }

    protected override void OnExitState()
    {
        Debug.Log("Exit Idle");
    }

    protected override void OnExitStateRender() { }

    protected override bool CanEnterState()
    {
        return base.CanEnterState();
    }

    protected override bool CanExitState(StateBehaviour nextState)
    {    
        return Machine.StateTime > 1f;
    }
}
