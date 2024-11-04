using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using UnityEngine;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class StateChanger : NetworkBehaviour
    {
        [SerializeField] private Collider col;

        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();

            if (Runner.Tick % 10 != 0) 
                return;
            
            if (PlayerRegistry.Where(p => col.bounds.Contains(p.transform.position)).Any())
            {
                NewGameManager.State.Server_SetState<PlayStateBehaviour>();
            }
        }
    }
}
