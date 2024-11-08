using Fusion;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using New_Neo_LT.Scripts.Map;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class NewGameManager : NetworkBehaviour
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
        // public static SoundManager Sm { get; private set; }
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
                // Sm = GetComponent<SoundManager>();
                // Vm = GetComponent<VoiceManager>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion
        
        #region NetworkBehaviour Events
        

        #endregion
        
        
        public Vector3 GetPregameSpawnPosition(int pIndex)
        {
            return pregameMapData.GetSpawnPosition(pIndex);
        }
        
        
        #region RPC Methods...
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_SellRelics(PlayerRef player, int[] inventoryIndices)
        {
            var playerCharacter = PlayerRegistry.GetPlayer(player);
            var playerInventory = playerCharacter.Inventory.ToArray();
            var goldPoint = 0;
            var renownPoint = 0;
            
            foreach (var index in inventoryIndices)
            {
                if (playerInventory[index] == -1)
                    continue;
                
                var relic = LegalThieves.RelicManager.Instance.GetRelicData(playerInventory[index]);
                goldPoint += relic.GetGoldPoint();
                renownPoint += relic.GetRenownPoint();
                playerCharacter.Inventory.Set(index, -1);
            }

            playerCharacter.AddGoldPoint(goldPoint);
            playerCharacter.AddRenownPoint(renownPoint);
        }

        #endregion
    }
}