using System;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public enum ECharacterType
    {
        None = -1,
        Archaeologist, 
        BusinessCultist,
        Linguist, 
        Shamanist,
        TypeCount
    }
    
    public enum EResultType
    {
        None = -1,
        Victory,
        Defeat,
        TypeCount
    }
    
    public class ResultUIController : MonoBehaviour
    {
        [SerializeField] private ResultUISlot[] slots;
        [SerializeField] private Camera[]       cameras;
        [SerializeField] private Animator[]     animators;
        
        public Camera GetCamera(int index) => cameras[index];
        
        // private bool _isInit;
        
        private static readonly int AnimResult = Animator.StringToHash("Result");
        private static readonly int AnimJob    = Animator.StringToHash("Job");
        private static readonly int AnimPlace  = Animator.StringToHash("Place");
        private static readonly int AnimSelected = Animator.StringToHash("Selected");

        private void Start()
        {
            InitCameras();
        }

        private void OnDisable()
        {
            foreach (var slot in slots)
            {
                slot.gameObject.SetActive(false);
            }
        }

        public void Init()
        {
            var sortedPlayers = PlayerRegistry.GetAllPlayers();
            
            // 승패 판정
            Array.Sort(sortedPlayers, (a, b) =>
            {
                var aTar = a.IsScholar ? a.GetRenownPoint : a.GetGoldPoint;
                var bTar = b.IsScholar ? b.GetRenownPoint : b.GetGoldPoint;
                return bTar.CompareTo(aTar);
            });
            
            for (var i = 0; i < sortedPlayers.Length; i++)
            {
                SetSlot(i, sortedPlayers[i]);
            }
        }

        private void SetSlot(int index, PlayerCharacter player)
        {
            var slot  = slots[index];
            var type  = player.GetJobIndex();
            
            SetResultAnimation(type, index);
            
            slot.gameObject.SetActive(true);
            slot.SetSlot(player.GetPlayerName(), player.GetGoldPoint, player.GetRenownPoint, cameras[type].targetTexture);
        }
        
        private void InitCameras()
        {
            foreach (var cam in cameras)
            {
                var textureSize = slots[0].GetRawImageSize();
                cam.targetTexture = new RenderTexture((int)textureSize.Item1, (int)textureSize.Item2, 24);
            }
            
            UIManager.Instance.jobChangerUI.SetRenderTexture(cameras);
        }
        
        public void SetResultAnimation(int jobIndex, int place)
        {
            animators[jobIndex].SetInteger(AnimPlace, place);
            animators[jobIndex].SetTrigger(AnimResult);
        }

        public void SetSelectAnimation(int jobIndex)
        {
            animators[jobIndex].SetInteger(AnimJob, jobIndex);
            animators[jobIndex].SetTrigger(AnimSelected);
        }
    }
}
