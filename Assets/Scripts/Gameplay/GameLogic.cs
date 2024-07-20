using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LegalThieves
{
    public enum EGameState
    {
        Waiting,
        Playing
    }
    public enum GoldOrRenown
    {
        Gold,
        Renown
    }

    public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef  playerPrefab;
        [SerializeField] private Transform         spawnpoint;
        [SerializeField] private Transform         spawnpointPivot;
        [SerializeField] private float             gametime;
        private int playerindex = 0;//마테리얼 적용용, 나중에 삭제
        
        [Networked] private TickTimer RemainTime { get; set; }

        [Networked] private TempPlayer Winner { get; set; }     //삭제 혹은 변경 예정

        [Networked, OnChangedRender(nameof(GameStateChanged))] private EGameState State { get; set; }

        [Networked, Capacity(4)] private NetworkDictionary<PlayerRef, TempPlayer> Players => default;

        [SerializeField] RelicDisplayer[] RelicBox = new RelicDisplayer[4];

        [SerializeField] CampPointUI campPointUI;

        [Networked, Capacity(120)] private NetworkArray<int> Relics { get; }

        [Networked, Capacity(40), OnChangedRender(nameof(CheckAllRoomExplained))] public NetworkArray<int> ExplainedRooms { get; } = MakeInitializer(Enumerable.Repeat(-1, 40).ToArray());

        #region Overrided user callback functions in NetworkBehaviour

        public override void Spawned()
        {
            Winner = null;
            State = EGameState.Waiting;
            UIManager.Singleton.SetWaitUI(State, Winner);
            Runner.SetIsSimulated(Object, true);

            AudioManager.instance.PlayJungleBgm(true);
            if (!HasStateAuthority) 
                return;
        }

        public override void FixedUpdateNetwork()
        {
            if (Players.Count < 1)
                return;

            if(State == EGameState.Playing)
                UIManager.Singleton.SetTimer((int)RemainTime.RemainingTime(Runner).GetValueOrDefault());

            if (!HasStateAuthority) 
                return;
            
            if (Runner.IsServer && State == EGameState.Waiting)
            {
                WaitingUpdate();
            }
            if (State == EGameState.Playing && !Runner.IsResimulation)
            {
                PlayingUpdate();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Runner.IsServer || Winner != null || other.attachedRigidbody == null ||
                !other.attachedRigidbody.TryGetComponent(out TempPlayer player)) 
                return;
            
            UnreadyAll();
            Winner = player;
            State = EGameState.Waiting;
        }

        #endregion
        
        #region GameLogic Methods

        private void GameStateChanged()
        {
            
        }

        private void PreparePlayers()
        {
            var spacingAngle = 360f / Players.Count;
            spawnpointPivot.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            foreach (var player in Players)
            {
                GetNextSpawnpoint(spacingAngle, out var position, out var rotation);
                player.Value.Teleport(position, rotation);
                player.Value.ResetCooldown();
            }
        }

        private void UnreadyAll()
        {
            foreach (var player in Players)
            {
                player.Value.isReady = false;
            }
        }

        private void GetNextSpawnpoint(float spacingAngle, out Vector3 position, out Quaternion rotation)
        {
            position = spawnpoint.position;
            rotation = spawnpoint.rotation;
            spawnpointPivot.Rotate(0f, spacingAngle, 0f);
        }
        
        //게임 상태가 Waiting일 때 FixedUpdateNetwork에서 돌아감
        private void WaitingUpdate()
        {
            //모든 플레이어 준비 상태 확인
            if (Players.All(player => player.Value.isReady))
            {
                State = EGameState.Playing;
                //플레이어 위치 스폰 포인트로 초기화
                PreparePlayers();
                RemainTime = TickTimer.CreateFromSeconds(Runner, gametime);
            }
            
        }
        
        //게임 상태가 Playing일 때 FixedUpdateNetwork에서 돌아감
        private void PlayingUpdate()
        {
            UIManager.Singleton.UpdateLeaderboard(Players.OrderByDescending(p => p.Value.Score).ToArray());
            
            if (!RemainTime.Expired(Runner)) 
                return;
            
            State = EGameState.Waiting;
            UnreadyAll();
        }

        // 라운드 종료 후 플레이어의 포인트를 계산
        private void CalculateTotalPoints()
        {
            foreach (RelicDisplayer box in RelicBox)
            {
                foreach (int invNum in box.GetAllRelics())
                {
                    //goldPoint += RelicManager.Singleton.GetTempRelicWithIndex(invNum).GoldPoint;
                    //renownPoint += RelicManager.Singleton.GetTempRelicWithIndex(invNum).RenownPoint;
                }
            }
            //renownpoint += ExplainPlayer.Count(a => a == 0) // 0~3, 1p~4p
        }
        
        // 방의 규명 확인 (플레이어가 유물을 캠프에 등록했을 때 호출될 예정)
        public void ExplainRoom(int roomID, RelicDisplayer relicDisplayer)
        {
            if (ExplainedRooms[roomID] != -1)
                return;
            int playerindex = 0;
            RelicBox.Count(a => a = relicDisplayer);
            foreach(var box in RelicBox)
            {
                if(box == relicDisplayer)
                {
                    ExplainedRooms.Set(roomID, playerindex);
                    return;
                }
                playerindex++;
            }
        }
        
        // 모든 방의 규명이 완료되었는지 확인 (ExplainRoom()의 안에서 규명이 확인되었을 때 호출될 예정)
        private void CheckAllRoomExplained()
        {
            if (ExplainedRooms.Contains(-1))
                return;
        }
        
        #endregion

        #region Join, Left Methods

        public void PlayerJoined(PlayerRef player)
        {
            if (!HasStateAuthority) 
                return;
            GetNextSpawnpoint(90f, out var position, out var rotation);
            var playerObject = Runner.Spawn(playerPrefab, position, rotation, player);
            Players.Add(player, playerObject.GetComponent<TempPlayer>());
            RelicBox[Players.Count - 1].owner = playerObject.GetComponent<TempPlayer>();
            campPointUI.AddPlayer(playerObject.GetComponent<TempPlayer>());
            if (playerindex >= 4)
                playerindex = 0;
            Debug.Log(playerindex);
            playerObject.GetComponent<TempPlayer>().SetClothMaterial(playerindex);

            playerindex++;
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (!HasStateAuthority || !Players.TryGet(player, out var playerBehaviour))
                return;

            Players.Remove(player);
            Runner.Despawn(playerBehaviour.Object);
        }

        #endregion
    }
}