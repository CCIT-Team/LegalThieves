using System.Collections;
using Fusion;
using Unity.VisualScripting;

namespace New_Neo_LT.Scripts.Game_Play.Game_State
{
    public enum EGameState
    {
        Waiting = 0,
        Playing,
        Finish,
        GameStateCount
    }
    
    public interface IState
    {
        public void Enter();
        public void Execute();
        public void Exit();
    }
    
    public class GameStateMachine
    {
        private IState _currentState;
        
        public void ChangeState(IState newState)
        {
            _currentState?.Exit();

            _currentState = newState;
            _currentState.Enter();
        }
        
        public IState GetCurrentState() => _currentState;
        
        public void Update()
        {
            _currentState?.Execute();
        }
    }

    public abstract class GameState : IState
    {
        protected NewGameManager Owner;

        public GameState(NewGameManager owner)
        {
            Owner = owner;
        }
        
        protected virtual void SetUI(bool isActive) { }
        
        public virtual void Enter()   { }
        public virtual void Execute() { }
        public virtual void Exit()    { }
    }
    
    public class WaitingState : GameState
    {
        public WaitingState(NewGameManager owner) : base(owner) { }

        protected override void SetUI(bool isActive)
        {
            // Set Waiting UI to Active
            // Set Player List UI to Active
            // Set Player Ready UI to Inactive
            // Set Player Joined UI to Active
            
        }
        
        public override void Enter()
        {
            SetUI(true);
            
            // If Host
            // Start Map Generation
            // Generate Rooms -> Set Room Connections -> Generate Streets -> Generate Relics
            
            // If Client
            // Try get Map Data
            // Set Map Data
            // Set Networked Objects Data (PlayerData, Relic ...) 스폰으로 생성된 오브젝트들은 알아서 동기화가 되나?
        }

        public bool IsAllPlayerJoined = false;

        public override void Execute()
        {
            // Check if all players are Joined and Ready
            if (IsAllPlayerJoined)
            {
                // Start Game
                Owner.ChangeGameState(EGameState.Playing);
            }
        }

        public override void Exit()
        {
            // Set Waiting UI to Inactive
            SetUI(false);
        }
    }
    
    public class PlayingState : GameState
    {
        public PlayingState(NewGameManager owner) : base(owner) { }
        
        public override void Enter()
        {
            // Set Playing UI to Active
            
            // Set profit quota timer 
        }

        public override void Execute()
        {
            // Check timer
            // if timer is 0
            //
            
            Owner.ChangeGameState(EGameState.Playing); 
        }

        public override void Exit()
        {
            
        }
    }
    
    public class FinishState : GameState
    {
        public FinishState(NewGameManager owner) : base(owner) { }
        
        public override void Enter()
        {
            
        }

        public override void Execute()
        {
            
        }

        public override void Exit()
        {
            // Disconnect all players
        }
    }
}
