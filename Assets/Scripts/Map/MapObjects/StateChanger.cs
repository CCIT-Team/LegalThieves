using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Game_Play.Game_State;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class StateChanger : NetworkBehaviour, global::IInteractable
    {

        public void OnServer_Interact(PlayerRef player)
        {
            if(!HasStateAuthority)
                return;

            NewGameManager.Instance.StartGame();
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }
    }
}
