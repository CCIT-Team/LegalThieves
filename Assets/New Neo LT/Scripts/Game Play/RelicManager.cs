using Fusion;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class RelicManager : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef[] relicPrefabs;
        
        [Networked, Capacity(100)] 
        private NetworkDictionary<int, Elements.Relic.Relic> Relics => default;
        
        public static RelicManager Instance;


        public override void Spawned()
        {
            base.Spawned();
            Instance = this;
            
            SpawnAllRelics();
        }

        public void SpawnAllRelics()
        {
            if (!Runner.IsServer)
                return;
            
            
        }
    }
}