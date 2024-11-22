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
        
        private bool _isInit;
        
        private static readonly int IsVictory = Animator.StringToHash("IsVictory");

        private void OnEnable()
        {
            if (_isInit)
                return;
            InitCameras();
            
            Init();
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
            
            animators[type].SetFloat(IsVictory, index);
            slot.gameObject.SetActive(true);
            slot.SetSlot(player.GetPlayerName(), player.GetGoldPoint, player.GetRenownPoint, cameras[type].targetTexture);
        }
        
        private void InitCameras()
        {
            _isInit = true;
            foreach (var cam in cameras)
            {
                var textureSize = slots[0].GetRawImageSize();
                cam.targetTexture = new RenderTexture((int)textureSize.Item1, (int)textureSize.Item2, 24);
            }
        }
    }
}
