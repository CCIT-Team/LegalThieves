using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using New_Neo_LT.Scripts.Map;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class NewGameManager : NetworkBehaviour//, IPlayerJoined, IPlayerLeft
    {
        [Header("Components & References")]
        [SerializeField] private NetworkPrefabRef    playerPrefab;
        [SerializeField] private float               playtime = 15;
        
        [Header("Player Color")]
        public Material[]                            playerClothMaterials;
        public Material[]                            playerHairMaterials;
        
        public static NewGameManager Instance { get; private set; }
        public static GameState State { get; private set; }
        public  ResourcesManager Rm { get; private set; }
        public  InterfaceManager Im { get; private set; }
        // public static VoiceManager Vm { get; private set; }
        
        public static float Playtime => Instance.playtime;
        
        public PregameStateMapData pregameMapData;
        public PlayStateMapData    playMapData;
        public PlayStateMapData    winMapData;
        
        [Networked, Capacity(4)] 
        public NetworkDictionary<PlayerRef, PlayerCharacter> Players => default;

        /*------------------------------------------------------------------------------------------------------------*/

        #region MonoBehaviour Events

        private void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                Im = GetComponent<InterfaceManager>();
                State = GetComponent<GameState>();
                Rm = GetComponent<ResourcesManager>();
                // Vm = GetComponent<VoiceManager>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
        
        #region NetworkBehaviour Events
        
        public override void Spawned()
        {
            base.Spawned();
            
            // UIManager.Instance.InitWaitUI();
        }

        #endregion

        #region Game Logic Methods

        public void Server_StartGame()
        {
            if(State.ActiveState is not PregameStateBehaviour)
                return;
            
            
        }
        
        public static bool HasPlayer(PlayerRef pRef)
        {
            return Instance.Players.ContainsKey(pRef);
        }
        
        public static PlayerCharacter GetPlayer(PlayerRef pRef)
        {
            return HasPlayer(pRef) ? Instance.Players.Get(pRef) : null;
        }

        #endregion
        
        private bool GetAvailable(out byte index)
        {
            if (Players.Count == 0)
            {
                index = 0;
                return true;
            }
            else if (Players.Count == 4)
            {
                index = default;
                return false;
            }

            byte[] indices = Players.OrderBy(kvp => kvp.Value.Index).Select(kvp => kvp.Value.Index).ToArray();
		
            for (int i = 0; i < indices.Length - 1; i++)
            {
                if (indices[i + 1] > indices[i] + 1)
                {
                    index = (byte)(indices[i] + 1);
                    return true;
                }
            }

            index = (byte)(indices[indices.Length - 1] + 1);
            return true;
        }
        
        public Vector3 GetPregameSpawnPosition(int pIndex)
        {
            return pregameMapData.GetSpawnPosition(pIndex);
        }
        
        public static void Server_Add(NetworkRunner runner, PlayerRef pRef, PlayerCharacter pObj)
        {
            Debug.Assert(runner.IsServer);

            if (Instance.GetAvailable(out byte index))
            {
                Instance.Players.Add(pRef, pObj);
                pObj.Server_Init(pRef, index);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning($"Unable to register player {pRef}", pObj);
#endif
            }
        }

        public static void Server_Remove(NetworkRunner runner, PlayerRef pRef)
        {
            Debug.Assert(runner.IsServer);
            Debug.Assert(pRef.IsRealPlayer);

            if (Instance.Players.Remove(pRef) == false)
            {
                Debug.LogWarning("Could not remove player from registry");
            }
        }

        #region Player Joined/Left Events
        //
        // public void PlayerJoined(PlayerRef player)
        // {
        //     if (!HasStateAuthority) 
        //         return;
        //     var randomIndex = UnityEngine.Random.Range(0, pregameMapData.SpawnPointCount);
        //     var playerChar = Runner.Spawn(playerPrefab, pregameMapData.GetSpawnPosition(randomIndex), quaternion.identity, player);
        //     Runner.SetPlayerObject(player, playerChar);
        //     
        //     Players.Add(player, playerChar.GetComponent<PlayerCharacter>());
        // }
        //
        // public void PlayerLeft(PlayerRef player)
        // {
        //     if(!HasStateAuthority || !Players.TryGet(player, out var playerInfo))
        //         return;
        //
        //     Players.Remove(player);
        //     
        //     Runner.TryGetPlayerObject(player, out var playerObject);
        //     Runner.Despawn(playerObject);
        // }
        //
        #endregion

        #region RelicTemp
        public void SellRelic(PlayerRef player, int RelicId)
        {
           //Relic re = RelicManager(����).GetRelic(RelicId)    //���� Ǯ���� �ش� ���� �޾ƿ�
           
        }
        #endregion
        #region RPC Methods...



        #endregion
    }
}