using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.UI;

namespace New_Neo_LT.Scripts.Map.MapObjects
{

    public class JobChange : NetworkBehaviour, global::IInteractable
    {
   
        public void OnServer_Interact(PlayerRef player)
        {
            var pc = PlayerRegistry.GetPlayer(player);
            
            
        }

        public void OnClient_Interact(PlayerRef player)
        {
            var pc = PlayerRegistry.GetPlayer(player);
            UIManager.Instance.jobChangerUI.gameObject.SetActive(true);
            UIManager.Instance.jobChangerUI.JobChangerInit(pc);

            //UIManager.Instance.relicPriceUI.SetWinPoint(pc.IsScholar);
        }
  
    }
}