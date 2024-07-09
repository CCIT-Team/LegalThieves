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

    public class GameLogic : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private NetworkPrefabRef  playerPrefab;
        [SerializeField] private Transform         spawnpoint;
        [SerializeField] private Transform         spawnpointPivot;
        [SerializeField] private Transform         relicSpawnpointsHolder;
        [SerializeField] private Vector3[]         relicSpawnpoints;
        [SerializeField] private NetworkPrefabRef  _relicPrefab;
        [SerializeField] private Relic             relicPrefab;

        [Networked] private Player Winner { get; set; }

        [Networked, OnChangedRender(nameof(GameStateChanged))] private EGameState State { get; set; }

        [Networked, Capacity(4)] private NetworkDictionary<PlayerRef, Player> Players => default;

        #region Overrided user callback functions in NetworkBehaviour
        
        public override void Spawned()
        {
            Winner = null;
            State = EGameState.Waiting;
            UIManager.Singleton.SetWaitUI(State, Winner);
            Runner.SetIsSimulated(Object, true);

            if (!HasStateAuthority) return;
            var t = relicSpawnpointsHolder.GetComponentsInChildren<Transform>();
            for (var i = 1; i < t.Length; i++)
            {
                Runner.Spawn(relicPrefab, t[i].position, t[i].rotation);
            }

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
        }

        private void OnTriggerEnter(Collider other)
        {
            if (Runner.IsServer && Winner == null && other.attachedRigidbody != null &&
                other.attachedRigidbody.TryGetComponent(out Player player))
            {
                UnreadyAll();
                Winner = player;
                State = EGameState.Waiting;
            }
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
        private void CalculateTotalPoints(){ }
        
        // 방의 규명 확인 (플레이어가 유물을 캠프에 등록했을 때 호출될 예정)
        private void ExplainRoom() { }
        
        // 모든 방의 규명이 완료되었는지 확인 (ExplainRoom()의 안에서 규명이 확인되었을 때 호출될 예정)
        private void CheckAllRoomExplained() { }
        
        #endregion

        #region Join, Left Methods

        public void PlayerJoined(PlayerRef player)
        {
            if (HasStateAuthority)
            {
                GetNextSpawnpoint(90f, out Vector3 position, out Quaternion rotation);
                NetworkObject playerObject = Runner.Spawn(playerPrefab, position, rotation, player);
                Players.Add(player, playerObject.GetComponent<Player>());
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            if (!HasStateAuthority)
                return;

            if (Players.TryGet(player, out Player playerBehaviour))
            {
                Players.Remove(player);
                Runner.Despawn(playerBehaviour.Object);
            }
        }

        #endregion
    }
}