using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using UnityEngine;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class PointAdder : NetworkBehaviour, global::IInteractable
    {
        [SerializeField] private int pointValue;
        [SerializeField] private bool isGold;
        
        public void OnServer_Interact(PlayerRef player)
        {
            var pc = PlayerRegistry.GetPlayer(player);
            if (isGold)
            {
                pc.AddGoldPoint(pointValue);
            }
            else
            {
                pc.AddRenownPoint(pointValue);
            }
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }
    }
}
