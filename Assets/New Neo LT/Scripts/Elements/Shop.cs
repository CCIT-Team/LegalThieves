using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.Relic;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements
{
    public class Shop : NetworkBehaviour, global::IInteractable
    {
        [SerializeField] private GameObject shopUI;
        
        private bool _isOpen;
        
        #region NetworkEvents
        public override void Spawned()
        {
            //Object.geta
        }

        public override void Render()
        {
        
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
        
        }
        #endregion


        void Sell(PlayerRef pref, RelicObject[] relics)
        {
            var gp = 0;
            var rp = 0;
            foreach(var relic in relics)
            {
                if (relic == null) continue;
                //gp += relic.goldPoint;
                //rp += relic.renownPoint;
            }
            //NewGameManager.GetPlayer(pref).goldPoint += gp;
            //NewGameManager.GetPlayer(pref).renownPoint += rp;
        }

        public void OnServer_Interact(PlayerRef player)
        {
            
        }

        public void OnClient_Interact(PlayerRef player)
        {
            shopUI.gameObject.SetActive(true);
        }
    }
}
