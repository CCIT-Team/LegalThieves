using ExitGames.Client.Photon.StructWrapping;
using Fusion;

using New_Neo_LT.Scripts.Game_Play.Game_State;
using New_Neo_LT.Scripts.Map;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class NewGameManager : NetworkBehaviour
    {
        [Header("Components & References")]
        [SerializeField] private NetworkPrefabRef    playerPrefab;
        [SerializeField] private float               playtime = 15;
        [SerializeField] private float               resttime = 30;
        [SerializeField] private float               loadtime = 1.5f;
        [SerializeField] private int                 rounds = 3;
        
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
        public static float Resttime => Instance.resttime;
        public static float Loadtime => Instance.loadtime;
        public static float Rounds => Instance.rounds;
        
        public PregameStateMapData pregameMapData;
        public PlayStateMapData    playMapData;
        public PlayStateMapData    winMapData;

        [Networked]
        int currentRound { get; set; } = 1;

        [Networked, Capacity(4), OnChangedRender(nameof(OnChangeJobButton))]
        public NetworkArray<bool> ButtonStateArray => default; // MakeInitializer(new bool[] { true, true, true, true });
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

        public void StartGame()
        {
            if (!PlayerRegistry.Any(pc => !pc.IsReady))
            {
                State.Server_SetState<LoadingStateBehaviour>();
                UIManager.Instance.readyStateUI.ToggleUI();
                UIManager.Instance.timerController.SetRound(currentRound);
            }
        }

        public bool RoundOver()
        {
            if (currentRound >= rounds)
                return true;

            currentRound++;
            UIManager.Instance.timerController.SetRound(currentRound);
            return false;
        }
        
        #region Job

        public void EnableJobButton(int i) 
        {
            //나가면 다시킴
            ButtonStateArray.Set(i, true);
        }

        public void OnChangeJobButton() // 갱신용
        {
            if (UIManager.Instance.jobChangerUI.gameObject.activeSelf == false) return;
            UIManager.Instance.jobChangerUI.JobChangerRenew(ButtonStateArray.ToArray());
        }
        #endregion
        
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
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPC_JobChange(PlayerRef player, Job job, int i)
        {
            //선택하면 비활성화
            var playerCharacter = PlayerRegistry.GetPlayer(player);
            ButtonStateArray.Set(playerCharacter.GetJobIndex(), true);
            playerCharacter.ChangeJob(job);
            playerCharacter.SetReady(true);
            ButtonStateArray.Set(i, false);
            
            // Debug.Log($"{ButtonStateArray.Get(0)}, {ButtonStateArray.Get(1)}, {ButtonStateArray.Get(2)},{ButtonStateArray.Get(3 )}");
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        public void RPC_SetPlayerReady(PlayerRef player, bool ready)
        {
            var playerCharacter = PlayerRegistry.GetPlayer(player);
            playerCharacter.SetReady(ready);
           
        }
        


        #endregion
    }
}