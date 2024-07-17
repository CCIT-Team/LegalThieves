using System.Linq;
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
        [Networked] private float startTime { get; set; }
        [Networked] public float remainTime { get; set; }

        [Networked] private TempPlayer Winner { get; set; }     //삭제 혹은 변경 예정

        [Networked, OnChangedRender(nameof(GameStateChanged))] private EGameState State { get; set; }

        [Networked, Capacity(4)] private NetworkDictionary<PlayerRef, TempPlayer> Players => default;

        [Networked, Capacity(4)] public NetworkArray<RelicDisplayer> RelicBox => default;

        [Networked, Capacity(120)] private NetworkArray<int> Relics { get; }

        [Networked, Capacity(40)] public NetworkArray<int> ExplainPlayer { get; } = MakeInitializer(Enumerable.Repeat(-1, 40).ToArray());

        #region Overrided user callback functions in NetworkBehaviour

        public override void Spawned()
        {
            Winner = null;
            State = EGameState.Waiting;
            UIManager.Singleton.SetWaitUI(State, Winner);
            Runner.SetIsSimulated(Object, true);
            startTime = Time.time;

            AudioManager.instance.PlayJungleBgm(true);
            if (!HasStateAuthority) 
                return;
        }

        public override void FixedUpdateNetwork()
        {
            if (Players.Count < 1)
                return;

            if (Runner.IsServer && State == EGameState.Waiting)
            {
                var areAllReady = Players.All(player => player.Value.isReady);

                if (areAllReady)
                {
                    Winner = null;
                    State = EGameState.Playing;
                    PreparePlayers();
                }
            }

            if (State == EGameState.Playing && !Runner.IsResimulation)
            {
                UIManager.Singleton.UpdateLeaderboard(Players.OrderByDescending(p => p.Value.Score).ToArray());
            }
            remainTime = 900- (Time.time - startTime);
            UIManager.Singleton.timer.text = (remainTime / 60 < 10 ? "0" + (int)remainTime / 60 : (int)remainTime / 60) + ":" + (remainTime % 60 < 10 ? "0" + (int)remainTime % 60 : (int)remainTime % 60);
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
            UIManager.Singleton.SetWaitUI(State, Winner);
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
            if (ExplainPlayer[roomID] != -1)
                return;
            int playerindex = 0;
            RelicBox.Count(a => a = relicDisplayer);
            foreach(var box in RelicBox)
            {
                if(box == relicDisplayer)
                {
                    ExplainPlayer.Set(roomID, playerindex);
                    return;
                }
                playerindex++;
            }
            CheckAllRoomExplained();
        }
        
        // 모든 방의 규명이 완료되었는지 확인 (ExplainRoom()의 안에서 규명이 확인되었을 때 호출될 예정)
        private void CheckAllRoomExplained()
        {
            if (ExplainPlayer.Contains(-1))
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
            RelicBox[Players.Count-1].SetOwner(playerObject.GetComponent<TempPlayer>());
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