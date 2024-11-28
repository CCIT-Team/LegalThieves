using Fusion;
using New_Neo_LT.Scripts.UI;

namespace New_Neo_LT.Scripts.Elements
{
    public class Shop : NetworkBehaviour, global::IInteractable
    {
        public void OnServer_Interact(PlayerRef player)
        {
            
        }

        public void OnClient_Interact(PlayerRef player)
        {
            UIManager.Instance.OpenShop();
        }
    }
}
