using Fusion;
using New_Neo_LT.Scripts.Game_Play;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class ColorChange : NetworkBehaviour, global::IInteractable
    {
        public void OnServer_Interact(PlayerRef player)
        {
            if (!HasStateAuthority)
                return;
            
            var pc = PlayerRegistry.GetPlayer(player);
            var prevIndex = pc.GetPlayerColor();
            pc.SetPlayerColor(prevIndex == 3 ? 0 : prevIndex + 1);
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }
    }
}
