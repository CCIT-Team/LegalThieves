using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements.Relic
{
    public class Relic : Interactable
    {
        [SerializeField] private GameObject visual;
        
        [Networked, OnChangedRender(nameof(OnIsActiveChange))] 
        public NetworkBool IsActivated { get; set; }


        private int _goldPoint;
        
        
        
        /*------------------------------------------------------------------------------------------------------------*/

        public override void Spawned()
        {
            base.Spawned();
            OnIsActiveChange();
        }

        private void OnIsActiveChange()
        {
            visual.SetActive(IsActivated);
        }

        public override bool CanInteract(PlayerRef player)
        {
            return true;
        }

        public override void Interact(NetworkObject interacter)
        {
            if(!CanInteract(interacter.Runner.LocalPlayer))
                return;
            
            Debug.Log($"{_goldPoint}");
        }
    }
}