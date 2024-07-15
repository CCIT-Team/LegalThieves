using System.Linq;
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

        //스폰된 유물이 담기는 네트워크배열. 네트워크배열에 관해서는 포톤 doc 참고.
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

        //모든 유물을 스폰하는 함수. 이후에 게임 state 구현되면 게임 준비 혹은 게임 시작 state에 호출되어 유물을 생성할 예정.
        //지금은 게임이 시작되면(호스트가 접속하면) 생성됨.
        //라운드 개념이 구현될 때 생성된 유물을 전부 디스폰하고 리스트를 초기화하는 작업이 구현될 필요가 있어보임.
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

        //
        public TempRelic GetTempRelicWithIndex(int index)
        {
            var tempRelic = SpawnedRelics[index];
            return tempRelic == null ? null : tempRelic;
        }
    }
}
