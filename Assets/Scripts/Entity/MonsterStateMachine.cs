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

public class MonsterStateMachine : NetworkBehaviour, IStateMachineOwner
{
    Monster monster;
    [SerializeField]
    GameObject states;

    private StateMachine<StateBehaviour> stateMachine;

    public void CollectStateMachines(List<IStateMachine> stateMachines)
    {
        var monsterStates = states.GetComponentsInChildren<StateBehaviour>();
        if (monsterStates[0].GetType() != typeof(SpawnState))
        {
            var spawnState = Array.Find(monsterStates, a => a.GetType() == typeof(SpawnState));
            monsterStates[monsterStates.IndexOf(spawnState)] = monsterStates[0];
            monsterStates[0] = spawnState;
        }

        stateMachine = new StateMachine<StateBehaviour>("Monster State",monsterStates);
        stateMachines.Add(stateMachine);
    }

    public override void Spawned()
    {
        monster = GetComponent<Monster>();
        if(Object.HasStateAuthority)
            SetTransitions();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(monster.monsterState == EMonseterState.Dead)
        {
            stateMachine.ForceActivateState<DeadState>();
        }
    }

    protected virtual void SetTransitions()
    {
        //Spawn -> Idle
        var state = Array.Find(stateMachine.States, a => a.IsType<SpawnState>());
        state.AddTransition(Array.Find(stateMachine.States,a => a.IsType<IdleState>()), () => state.Machine.StateTime >= 5);

        //Idle -> ???
        state = Array.Find(stateMachine.States, a => a.IsType<IdleState>());
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

        //Attack -> Idle,Chase
        state = Array.Find(stateMachine.States, a => a.IsType<AttackState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<IdleState>()), () => state.Machine.StateTime >= 1.5 + 1.5  && state.Machine.PreviousState == state.Machine.GetState<IdleState>());
        state.AddTransition(Array.Find(stateMachine.States, a => a.IsType<ChaseState>()), () => state.Machine.StateTime >= 1.5 + 1.5 && state.Machine.PreviousState == state.Machine.GetState<ChaseState>());

    }
}
