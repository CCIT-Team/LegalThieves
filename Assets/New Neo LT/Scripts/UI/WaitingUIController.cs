using TMPro;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class WaitingUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerCountText;


        public void Init()
        {
            
        }
        
        public void SetPlayerCount(int playerCount)
        {
            playerCountText.text = playerCount.ToString() + " / 4";
        }
    }
}