using System;
using UnityEngine;

namespace MonsterStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public State currentState;
        public void Initialize(State startingState)
        {
            currentState = startingState;
            startingState.Enter();
        }

        

        public void ChangeState(State newState)
        {
            currentState.Exit();
        
            currentState = newState;
            newState.Enter();
        }
    }
}
