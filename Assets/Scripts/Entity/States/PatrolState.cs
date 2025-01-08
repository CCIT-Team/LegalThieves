using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using LegalThieves;
using New_Neo_LT.Scripts.Game_Play;

public class PatrolState : MonsterStateBase
{
    float patrolTime = 0;
    protected override void OnInitialize()
	{
        
    }
    protected override void OnDeinitialize(bool hasState) { }

    protected override void OnEnterState()
    {
        monster.agent.isStopped = false;
        monster.target = PlayerRegistry.GetRandom().transform;
        patrolTime = Random.Range(0, 10);
    }
    protected override void OnFixedUpdate()
    {

    }
    protected override void OnExitState() { }

    protected override void OnEnterStateRender() { }

    protected override void OnRender() { }
    protected override void OnExitStateRender() { }

    protected override bool CanExitState(StateBehaviour nextState)
    {
        return Machine.StateTime >= patrolTime;
    }
}
