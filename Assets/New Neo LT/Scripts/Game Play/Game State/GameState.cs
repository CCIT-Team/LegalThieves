using System.Collections.Generic;
using Fusion;
using Fusion.Addons.FSM;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public class GameState : NetworkBehaviour, IStateMachineOwner
    {
        public StateBehaviour ActiveState  => _stateMachine.ActiveState;
        public bool           AllowInput   => _stateMachine.ActiveStateId == winState.StateId || _stateMachine.ActiveStateId == playState.StateId;
        public bool           IsInGame     => _stateMachine.ActiveStateId == playState.StateId;
        
        [Networked] private TickTimer         Delay           { get; set; } 
        [Networked] private int               DelayedStateId  { get; set; }
        
        [Header("Game State References")]
        public PregameStateBehaviour          pregameState;
        public PlayStateBehaviour             playState;
        public WinStateBehaviour              winState;
        public LoadingStateBehaviour          loadingState;
        public EndStateBehaviour              endState;

        // PreGame (-> Loading -> Play -> Loading -> Win) * 3 -> End

        private StateMachine<StateBehaviour>  _stateMachine;

        public override void FixedUpdateNetwork()
        {
            if (DelayedStateId >= 0 && Delay.ExpiredOrNotRunning(Runner))
            {
                _stateMachine.ForceActivateState(DelayedStateId);
                DelayedStateId = -1;
            }
        }
        
        public float GetRemainingTime()
        {
            return Delay.RemainingTime(Runner) ?? -1;
        }
        
        public void Server_SetState<T>() where T : StateBehaviour
        {
            Assert.Check(HasStateAuthority, "호스트만 상태를 변경할 수 있습니다.");
            
            Delay = TickTimer.None;
            DelayedStateId = _stateMachine.GetState<T>().StateId;
#if UNITY_EDITOR
            Debug.Log($"{nameof(T)}로 상태 변경");
#endif
        }
        
        public void Server_DelaySetState<T>(float delay) where T : StateBehaviour
        {
            Assert.Check(HasStateAuthority, "호스트만 상태를 변경할 수 있습니다.");
            
#if UNITY_EDITOR
            Debug.Log($"{delay}초 후 {nameof(T)}로 상태 변경 예약");
#endif
            Delay = TickTimer.CreateFromSeconds(Runner, delay);
            DelayedStateId = _stateMachine.GetState<T>().StateId;
        }

        public void CollectStateMachines(List<IStateMachine> stateMachines)
        {
            _stateMachine = new StateMachine<StateBehaviour>("Game_State", pregameState, playState, winState, loadingState, endState);
            stateMachines.Add(_stateMachine);
        }
    }
}
