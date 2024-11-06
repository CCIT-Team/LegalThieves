using Fusion;
using New_Neo_LT.Scripts.Relic;
using UnityEngine;

namespace LegalThieves
{
    public class RelicManager : NetworkBehaviour
    {
        public static RelicManager Instance;

        [Header("Components")]
        [SerializeField] private NetworkPrefabRef    relicPrefab;
        [SerializeField] private Transform           relicPool;

        [Networked, Capacity(200)]
        NetworkLinkedList<RelicObject> Relics => default;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            for (var i = 0; i < 5; i++)
            {
                SpawnRelic(new Vector3(0, 1, i));
            }
        }

        //생성, 드랍, 진열 -> 스폰
        //줍기 -> 디스폰
        //팔기 -> 포인트


        public void SpawnRelic(Vector3 position = default)
        {
            var relic = Runner.Spawn(relicPrefab, position).GetComponent<RelicObject>();
            
            Relics.Add(relic);
        }

        public RelicObject GetRelicData(int index)
        {
            return Relics[index];
        }
        
        public int GetRelicIndex(RelicObject relic)
        {
            return Relics.IndexOf(relic);
        }
    }
}
