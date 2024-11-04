using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements.Relic
{
    public class Relic : NetworkBehaviour
    {
        [SerializeField] private GameObject visual;
        
        [Networked] public int Scroe { get; set; }
        [Networked] public int Type { get; set; }
        [Networked, OnChangedRender(nameof(SetActiveSelf))] public bool IsActivated { get; set; }
        
        
        
        /*------------------------------------------------------------------------------------------------------------*/

        public override void Spawned()
        {
            
        }

        private void SetActiveSelf()
        {
            visual.SetActive(IsActivated);
        }

        public bool TryInteraction(Character character)
        {
            // var index = character.gameObject.GetComponent<PlayerInventory>().TryInsert(this);
            // if (index == -1)
            //     return false;
            // IsActivated = false;
            return true;
        }
    }
}