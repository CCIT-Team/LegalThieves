using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using System.ComponentModel;

public class AttackState : MonsterStateBase
{
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.agent.isStopped = true;
        monster.attackColider.gameObject.SetActive(true);
    }
    protected override void OnFixedUpdate()
    {

    }
    protected override void OnExitState()
    {
        monster.attackColider.gameObject.SetActive(false);
    }

    protected override void OnEnterStateRender()
    {
        monster.animator.SetTrigger("Attack");
    }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }
}
