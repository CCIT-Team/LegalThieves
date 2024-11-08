using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.Relic;
using New_Neo_LT.Scripts.UI;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements
{
    public class Shop : NetworkBehaviour, global::IInteractable
    {
        public void OnServer_Interact(PlayerRef player)
        {
            
        }

        public void OnClient_Interact(PlayerRef player)
        {
            UIManager.Instance.shopController.gameObject.SetActive(true);
        }
    }
}
