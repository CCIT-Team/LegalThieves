using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Elements.Relic
{
    public class RelicPool : NetworkBehaviour
    {
        [SerializeField] private int relicCount = 5;
        [SerializeField] private NetworkPrefabRef relicPrefab;
        
        public static RelicPool Instance { get; private set; }
        public Relic[] relics;

        public override void Spawned()
        {
            Instance = this;
            relics = new Relic[relicCount];
            
            
            if(!Runner.IsServer)
                return;
            
            
            //for (var i = 0; i < relicCount; i++)
            //{
            //    relics[i] = Runner.Spawn(relicPrefab, new Vector3(0,0,i), Quaternion.identity).GetComponent<Relic>();
            //    relics[i].IsActivated = true;
            //    relics[i].Scroe = Random.Range(1, 10);
            //    relics[i].Type = Random.Range(0, 5);
            //}
            
        }
    }
}
