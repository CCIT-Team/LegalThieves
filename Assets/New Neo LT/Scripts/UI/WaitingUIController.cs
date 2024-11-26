using System;
using New_Neo_LT.Scripts.Game_Play;
using TMPro;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class WaitingUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text  playerCountText;
        [SerializeField] private LoadingUI loadingUI;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            SetPlayerCount(PlayerRegistry.Count);
        }

        public void StartEndOfProgress()
        {
            loadingUI.StartEndOfProgress();
        }
        
        public void SetPlayerCount(int playerCount)
        {
            playerCountText.text = playerCount.ToString() + " / 4";
        }
    }
}