using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using UnityEngine;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class StateChanger : NetworkBehaviour, global::IInteractable
    {
        private global::IInteractable _interactableImplementation;

        public void OnServer_Interact(PlayerRef player)
        {
            if(!HasStateAuthority)
                return;

            NewGameManager.State.Server_SetState<PlayStateBehaviour>();
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }
    }
}
