using Fusion;
using Fusion.Addons.Physics;
using New_Neo_LT.Scripts.Elements.Relic;
using New_Neo_LT.Scripts.Game_Play;
using UnityEngine;
using RelicManager = LegalThieves.RelicManager;
using UIManager = New_Neo_LT.Scripts.UI.UIManager;

namespace New_Neo_LT.Scripts.Relic
{
    public class RelicObject : NetworkBehaviour, global::IInteractable
    {
        [SerializeField] private MeshRenderer visual;
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private NetworkRigidbody3D networkRigidbody;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip[] sounds;

        public int RelicID => RelicManager.Instance.GetRelicIndex(this);

        [SerializeField]
        private int typeIndex = 0;

        [SerializeField]
        private ERelic relicType { get; set; }
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
            
            if(HasStateAuthority) 
                InitRelic();
        }
        
        private void InitRelic()
        {
            var tempPoint = Random.Range(501, 1000);
            if(relicType == ERelic.Gold)
                SetPoints(tempPoint, 1000 - tempPoint);
            else
                SetPoints(1000 - tempPoint, tempPoint);
            //SetRelicType();
            SetRelicScale();
        }
        
        //private void SetVisual()
        //{
        //    visual.GetComponent<MeshFilter>().mesh = LegalThieves.RelicManager.Instance.GetRelicMesh(TypeIndex);
        //    visual.GetComponent<MeshRenderer>().materials = LegalThieves.RelicManager.Instance.GetRelicMaterial(TypeIndex);
        //}

        public void OnServer_Interact(PlayerRef player)
        {
            if (!PlayerRegistry.GetPlayer(player).GetRelic(LegalThieves.RelicManager.Instance.GetRelicIndex(this)))
                return;

            IsActivated = false;
        }

        public void OnClient_Interact(PlayerRef player)
        {
            
        }

        public void OnThrowAway(PlayerRef player)
        {
            var ownerTf = PlayerRegistry.GetPlayer(player).GetCamTarget();
            var spawnPoint = ownerTf.forward + ownerTf.position;

            transform.position = spawnPoint;
            IsActivated = true;
        }

        public void OnApplyRelic(PlayerRef player)
        {
            var pc = PlayerRegistry.GetPlayer(player);
            pc.AddGoldPoint(GoldPoint);
            pc.AddRenownPoint(RenownPoint);
        }

        private void OnIsActiveChange()
        {
            if(visual == null)
            {
                foreach(MeshRenderer renderer in GetComponentsInChildren<MeshRenderer>())
                    renderer.enabled = IsActivated;
            }
            else
                visual.enabled = IsActivated;

            boxCollider.enabled = IsActivated;
            audioSource.PlayOneShot(sounds[IsActivated ? 0 : 1]);
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
        
        public (int, int) GetPoints()
        {
            return (GoldPoint, RenownPoint);
        }
        
        public int GetTypeIndex()
        {
            return typeIndex;
        }

        public ERelic GetRelicType()
        {
            return relicType;
        }
        private void SetRelicType()
        {
            if (GoldPoint > RenownPoint)
            {
                relicType = ERelic.Gold;
            }
            else
            {
                relicType = ERelic.Renown;
            }
        }
        private void SetRelicScale()
        {
            float factor = 0;
            if (relicType == ERelic.Gold)
            {
                factor = GoldPoint / 500f;
            }
            else
            {
                factor = RenownPoint / 500f;
            }

            transform.localScale *= factor;
        }
    }
}
