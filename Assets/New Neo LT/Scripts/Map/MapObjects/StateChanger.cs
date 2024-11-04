using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using UnityEngine;

namespace New_Neo_LT.Scripts.Map.MapObjects
{
    public class StateChanger : NetworkBehaviour
    {
        [SerializeField] private Collider col;

        

        private void OnTriggerEnter(Collider other)
        {
            if(!HasInputAuthority)
                return;
            if(other.CompareTag("Player"))
                NewGameManager.State.Server_SetState<PlayStateBehaviour>();
        }
    }
}
