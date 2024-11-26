using TMPro;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class WaitingUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text  playerCountText;
        [SerializeField] private LoadingUI loadingUI;

        public float endTime;
        
        public void Init()
        {
            
        }

        public void StartEndofProgress()
        {
            loadingUI.StartEndOfProgress();
        }
        
        public void SetPlayerCount(int playerCount)
        {
            playerCountText.text = playerCount.ToString() + " / 4";
        }
    }
}