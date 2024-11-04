using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class GameState : NetworkBehaviour, IStateMachineOwner
    {
        public StateBehaviour ActiveState  => _stateMachine.ActiveState;
        public bool           AllowInput   => _stateMachine.ActiveStateId == playState.StateId || _stateMachine.ActiveStateId == pregameState.StateId;
        public bool           IsInGame     => _stateMachine.ActiveStateId == playState.StateId;
        
        [Networked] private TickTimer         Delay           { get; set; } 
        [Networked] private int               DelayedStateId  { get; set; }
        
        [Header("Game State References")]
        public PregameStateBehaviour          pregameState;
        public PlayStateBehaviour             playState;
        public WinStateBehaviour              winState;
        
        private StateMachine<StateBehaviour>  _stateMachine;

        public void FixedUpdate()
        {
            // if (DelayedStateId >= 0 && Delay.ExpiredOrNotRunning(Runner))
            // {
            //     _stateMachine.ForceActivateState(DelayedStateId);
            //     DelayedStateId = -1;
            // }
        }
        
        public void Server_SetState<T>() where T : StateBehaviour
        {
            Assert.Check(HasInputAuthority, "호스트만 상태를 변경할 수 있습니다.");
            
            Delay = TickTimer.None;
            DelayedStateId = _stateMachine.GetState<T>().StateId;
        }
        
        public void Server_DelaySetState<T>(float delay) where T : StateBehaviour
        {
            Assert.Check(HasInputAuthority, "호스트만 상태를 변경할 수 있습니다.");
            
#if UNITY_EDITOR
            Debug.Log($"{delay}초 후 {nameof(T)}로 상태 변경 예약");
#endif
            Delay = TickTimer.CreateFromSeconds(Runner, delay);
            DelayedStateId = _stateMachine.GetState<T>().StateId;
        }

        public void CollectStateMachines(List<IStateMachine> stateMachines)
        {
            _stateMachine = new StateMachine<StateBehaviour>("Game_State", pregameState, playState, winState);
            stateMachines.Add(_stateMachine);
        }
    }
}
