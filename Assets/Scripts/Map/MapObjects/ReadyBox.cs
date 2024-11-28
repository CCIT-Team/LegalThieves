using Fusion;
using New_Neo_LT.Scripts.Game_Play;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class ReadyBox : NetworkBehaviour, global::IInteractable
    {
        public void OnServer_Interact(PlayerRef player)
        {
            if(!HasStateAuthority)
                return;
            
            var p = PlayerRegistry.GetPlayer(player);
            p.SetReady(!p.IsReady);
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }
    }
}