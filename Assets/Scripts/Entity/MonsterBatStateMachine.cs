using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Addons.FSM;
using Fusion;
using System;
using Fusion.Addons.KCC;
using ExitGames.Client.Photon.StructWrapping;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;

public class MonsterBatStateMachine : MonsterStateMachine
{
    public int currentRelic = -1;
    protected override void SetTransitions()
    {
        //Spawn -> Idle
        var state = Array.Find(stateMachine.States, a => a.IsType<SpawnState>());
        state.AddTransition(Array.Find(stateMachine.States,a => a.IsType<IdleState>()), () => state.Machine.StateTime >= 5);

        //Idle -> ???
        state = Array.Find(stateMachine.States, a => a.IsType<IdleState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<PatrolState>()), () => stateMachine.StateTime >= UnityEngine.Random.Range(0, 3));
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<ChaseState>()), () => monster.CheckPlayersInRange());
        //state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<AttackState>()), () => CheckEnumChanged(EMonseterState.Attack));

        //Chase -> ???
        state = Array.Find(stateMachine.States, a => a.IsType<ChaseState>());
        //state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => CheckEnumChanged(EMonseterState.Idle));
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<AttackState>()), () => monster.CanAttackPlayer());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<ReturnState>()), () => !monster.CheckPlayersInRange());

        //Return -> Idle
        state = Array.Find(stateMachine.States, a => a.IsType<ReturnState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => Vector3.SqrMagnitude(monster.spawnPosition - monster.transform.position) <= 2);

        //Attack -> Escape,Idle,Chase
        state = Array.Find(stateMachine.States, a => a.IsType<AttackState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<EscapeState>()), () => currentRelic != -1);
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => state.Machine.StateTime >= 1.5 + 1.5  && state.Machine.PreviousState == state.Machine.GetState<IdleState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<ChaseState>()), () => state.Machine.StateTime >= 1.5 + 1.5 && state.Machine.PreviousState == state.Machine.GetState<ChaseState>());

        //Patrol -> Idle
        state = Array.Find(stateMachine.States, a => a.IsType<PatrolState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => true);

        //Escape -> Idle
        state = Array.Find(stateMachine.States, a => a.IsType<EscapeState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => true);

    }
}
