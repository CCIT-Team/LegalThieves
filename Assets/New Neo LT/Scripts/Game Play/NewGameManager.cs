using Fusion;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public struct PlayerInfo : INetworkStruct
    {
        [Networked, Capacity(24)] public string             PlayerName => default;
        [Networked, Capacity(10)] public NetworkArray<int>  Relics       => default;

        [Networked] public bool                             IsWinByGold  { get; set; }
        [Networked] public int                              GoldPoint    { get; set; }
        [Networked] public int                              RenownPoint  { get; set; }
    }
    
    public class NewGameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [Header("Components & References")]
        [SerializeField] private NetworkPrefabRef    playerPrefab;
        [SerializeField] private Transform           spawnPoint;
        
        [Networked, Capacity(4)] public NetworkDictionary<PlayerRef, PlayerInfo> Players => default;
        
        private GameStateMachine _stateMachine;
        
        private GameState[] _states = new GameState[(int)EGameState.GameStateCount];

        private void InitializeGameStates()
        {
            _states[(int)EGameState.Waiting]   = new WaitingState(this);
            _states[(int)EGameState.Playing]   = new PlayingState(this);
            _states[(int)EGameState.Finish]    = new FinishState(this);
            
            _stateMachine = new GameStateMachine();
            _stateMachine.ChangeState(_states[(int)EGameState.Waiting]);
        }

        public void ChangeGameState(EGameState newState)
        {
            _stateMachine.ChangeState(_states[(int)newState]);
        }

        /*------------------------------------------------------------------------------------------------------------*/
        
        #region NetworkBehaviour Events
        
        public override void Spawned()
        {
            if(!HasStateAuthority)
                return;
            Runner.SetIsSimulated(Object, true);
        }
        
        // public override void FixedUpdateNetwork()
        // {
        //     
        // }
        //
        // public override void Render()
        // {
        //     
        // }

        #endregion

        #region Game Logic Methods

        //게임 상태가 변경되었을 때 호출되는 함수
        private void OnGameStateChanged(NetworkBehaviourBuffer previousState)
        {
            //변경 이전값 : previousState, 변경 이후값 : State
            //게임 상태가 변경되었을 때 처리할 로직을 작성 예) UI 변경 등..
        }

        #endregion

        #region Player Joined/Left Events

        public void PlayerJoined(PlayerRef player)
        {
            if (HasStateAuthority)
            {
                var playerChar = Runner.Spawn(playerPrefab, spawnPoint.position, spawnPoint.rotation, player);
                Runner.SetPlayerObject(player, playerChar);
                
                Players.Add(player, new PlayerInfo
                {
                    PlayerName = PlayerPrefs.GetString("Photon.Menu.Username"),
                    GoldPoint = 0,
                    RenownPoint = 0
                });
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if(!HasStateAuthority || !Players.TryGet(player, out var playerInfo))
                return;

            Players.Remove(player);
            
            Runner.TryGetPlayerObject(player, out var playerObject);
            Runner.Despawn(playerObject);
        }

        #endregion

        #region RPC Methods...

        

        #endregion
    }
}