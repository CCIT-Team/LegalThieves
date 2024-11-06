using Fusion;
using Fusion.Addons.Physics;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.Relic
{
    public class RelicObject : NetworkBehaviour, global::IInteractable
    {
        [SerializeField] private GameObject visual;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private NetworkRigidbody3D networkRigidbody;
        
        public bool IsActive => IsActivated;
        
        [Networked]
        private int GoldPoint { get; set; }
        [Networked]
        private int RenownPoint {get; set; }
    
        [Networked, OnChangedRender(nameof(OnIsActiveChange))]
        private bool IsActivated { get; set; }
        
        public override void Spawned()
        {
            base.Spawned();
            IsActivated = true;
            
            if(!HasStateAuthority)
                return;
            GoldPoint = Random.Range(1, 10);
        }
        
        public string GetInteractPrompt()
        {
            return "Pick up";
        }

        public void OnInteract(PlayerRef player)
        {
            // if (!HasStateAuthority) 
            //     return;
            
            if (PlayerRegistry.GetPlayer(player).SetSlot(LegalThieves.RelicManager.Instance.GetRelicIndex(this)))
            {
                IsActivated = false;
            }
        }
        
        public void OnThrowAway(PlayerRef player)
        {
            if (!HasStateAuthority) 
                return;

            var ownerTf = PlayerRegistry.GetPlayer(player).GetCamTarget();
            var spawnPoint = ownerTf.forward + ownerTf.position;
            
            transform.position = spawnPoint;
            IsActivated = true;
        }

        public void OnApplyRelic(NetworkRunner runner)
        {
            if (!HasStateAuthority) 
                return;
        }

        private void OnIsActiveChange()
        {
            visual.SetActive(IsActivated);
            boxCollider.enabled = IsActivated;
            networkRigidbody.RBIsKinematic = !IsActivated;
        }
        
        public void SetPoints(int goldPoint, int renownPoint)
        {
            GoldPoint = goldPoint;
            RenownPoint = renownPoint;
        }
        
        public int GetGoldPoint()
        {
            return GoldPoint;
        }
        
        public int GetRenownPoint()
        {
            return RenownPoint;
        }
    }
}
