using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class DeadState : MonsterStateBase
{
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.agent.isStopped = true;
    }
    protected override void OnFixedUpdate() { }
    protected override void OnExitState() { }

    protected override void OnEnterStateRender()
    {
        monster.animator.SetTrigger("Die");
        //monster.animator.SetBool("Dead", true);
    }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }
}
