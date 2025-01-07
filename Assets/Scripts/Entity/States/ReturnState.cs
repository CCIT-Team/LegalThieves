using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;

public class ReturnState : MonsterStateBase
{
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.target = null;
        monster.agent.isStopped = false;
    }
    protected override void OnFixedUpdate()
    {

    }
    protected override void OnExitState() { }

    protected override void OnEnterStateRender()
    {
        monster.animator.SetTrigger("Return");
    }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }
}
