using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LegalThieves
{
    public class RelicManager : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private NetworkPrefabRef[]  tempRelicPrefab;
        [SerializeField] private TempRoom[]          tempRooms;
        [SerializeField] private Transform           relicPool;

        [Space]
        [Header("Setup")]
        [SerializeField] private float depthMuliple = 1f;

        [Networked, Capacity(200)] private NetworkArray<TempRelic> SpawnedRelics { get; }

        public static RelicManager Singleton
        {
            get => _singleton;
            private set
            {
                if (value == null) _singleton = null;
                else if (_singleton == null) _singleton = value;
                else if (_singleton != value)
                {
                    Destroy(value);
                    Debug.LogError($"{nameof(RelicManager)}는(은) 단 한번만 인스턴싱되어야 합니다!");
                }
            }
        }
        private static RelicManager _singleton;

        private void Awake()
        {
            Singleton = this;
        }

        private void OnDestroy()
        {
            if (Singleton == this)
                Singleton = null;
        }

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            SpawnRelics();
        }

        private void SpawnRelics()
        {
            var relicCount = 0;
            for (var i = 0; i < tempRooms.Length; i++)
            {
                var relicSpawnPoint = tempRooms[i].tempRelicSpawnPoints;
                foreach (var t in relicSpawnPoint)
                {
                    var randomTempRelic = tempRelicPrefab[Random.Range(0, tempRelicPrefab.Length)];
                    var networkObjRelic = Runner.Spawn(randomTempRelic, t.position, t.rotation);
                    var tempRelic = networkObjRelic.GetComponent<TempRelic>();
                    
                    networkObjRelic.transform.SetParent(relicPool);
                    
                    tempRelic.relicNumber = relicCount;
                    tempRelic.RoomNum = i;
                    
                    
                    
                    SpawnedRelics.Set(relicCount, tempRelic);
                    relicCount++;
                }
            }
            
        }

        public TempRelic GetTempRelicWithIndex(int index)
        {
            var networkObject = SpawnedRelics.Get(index);
            return networkObject;
        }
    }
}
