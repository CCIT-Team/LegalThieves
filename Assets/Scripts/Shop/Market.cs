using Fusion;
using New_Neo_LT.Scripts.UI;

namespace Shop
{
    public class Market : NetworkBehaviour, IInteractable
    {
        public void OnServer_Interact(PlayerRef player)
        {
            
        }

        public void OnClient_Interact(PlayerRef player)
        {
            // Open market UI
            // UIManager.Instance.SetActiveUI(UIType.MarketUIController, true);
            UIManager.Instance.OpenMarket();
        }
    }
}