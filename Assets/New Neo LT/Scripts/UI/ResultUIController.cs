using System;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public enum CharacterType
    {
        None = -1,
        Archaeologist, 
        BusinessCultist,
        Linguist, 
        Shamanist,
        TypeCount
    }
    
    public class ResultUIController : MonoBehaviour
    {
        [SerializeField] private ResultUISlot[] slots;
        [SerializeField] private Camera[]       cameras;

        private void Start()
        {
            InitCameras();
        }

        private void OnEnable()
        {
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
        }
    }
}
