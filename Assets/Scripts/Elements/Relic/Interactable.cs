using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements.Relic
{
    public abstract class Interactable : NetworkBehaviour
    {

        public abstract bool CanInteract(PlayerRef player);
        
        public abstract void Interact(NetworkObject interacter);
    }
}
