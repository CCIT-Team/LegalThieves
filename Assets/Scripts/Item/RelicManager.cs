using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using static RelicCreation;
using Random = UnityEngine.Random;

namespace LegalThieves
{
    public class RelicManager : NetworkBehaviour
    {
        [Header("Components")]
        [SerializeField] private NetworkPrefabRef[]  tempRelicPrefab;
        [SerializeField] private TempRoom[]          tempRooms;
        [SerializeField] private Transform           relicPool;
        [SerializeField] private NavMeshBaker navMeshBaker; // NavMeshBaker 참조 추가

        [Space]
        [Header("Setup")]
        [SerializeField] private float depthMuliple = 1f;

        [Networked, Capacity(200)] private NetworkArray<TempRelic> SpawnedRelics { get; }

        private static bool isNavMeshBaked = false; // static으로 변경하여 전역에서 접근 가능하도록 수정

        [System.Serializable]//7.15일 유물생성 추가 중
        public struct YearProbability
        {
            public int relicdepth;
            public int probability;
        }
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
        [System.Serializable]//7.15일 유물생성 추가 중
        public struct ProbabilityTable
        {
            public List<YearProbability> probabilities;
        }

        [System.Serializable]//7.15일 유물생성 추가 중
        public class Relices
        {
            public RelicesType type; // 유물 타입
            public int depth; // 티어
            public int goldPoint; // 골드 포인트
            public int renownPoint; // 명성 포인트
            public int RoomNum; // 방 ID
            public int excavation; // 발굴도->9.25
            public int damage; // 훼손도->9.25

            public enum RelicesType { NormalRelic, GoldRelic, RenownRelic } // 유물 타입 열거형
        }
        

        [System.Serializable]//7.15일 유물생성 추가 중
        public class RelicesList
        {
            public Relices[] Relicess; // csv파일읽고 생성된 유물 배열
        }

        public int RoomNum; // 방 ID 7.15일 유물생성 추가 중
        public TextAsset relicPrefabsData; // 유물 CSV 파일 데이터 7.15일 유물생성 추가 중
        public TextAsset probabilityTableData; // 확률 테이블 CSV 파일 데이터 7.15일 유물생성 추가 중
        public List<Relices> relicess = new List<Relices>(); // 직접 유물 리스트 객체 7.15일 유물생성 추가 중
        public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // 확률 테이블 리스트 

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
        void Start()//7.15일 유물생성 추가 중
        {
            ReadCSV(); // CSV 파일 읽기
            ReadProbabilityCSV(); // 확률 테이블 CSV 파일 읽기
            Debug.Log("CSV 데이터 로드 완료: " + relicess.Count + "명의 플레이어 로드됨");

            //베이크가 아직 수행되지 않았다면 네비게이션 메쉬 빌드
            if (!isNavMeshBaked)
            {
                navMeshBaker.BakeNavMesh();
                isNavMeshBaked = true; // 베이크가 완료되었음을 플래그로 표시
                Debug.Log("네비게이션 메쉬가 LateUpdate에서 빌드외었습니다.");
            }

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
        private void SpawnRelics() // 유물 생성 메서드 수정
        {
            var relicCount = 0;

            // 1. 모든 스폰 포인트 탐색하여 가중치 수집
            List<(Transform spawnPoint, float areaCost)> spawnPointsWithCosts = new List<(Transform, float)>();

            for (var i = 0; i < tempRooms.Length; i++)
            {
                var relicSpawnPoints = tempRooms[i].tempRelicSpawnPoints;

                foreach (var t in relicSpawnPoints)
                {
                    Ray ray = new Ray(t.position, Vector3.down);
                    RaycastHit raycastHit;

                    if (Physics.Raycast(ray, out raycastHit, 10f)) // 10m 아래로 레이캐스트 진행
                    {
                        NavMeshHit navMeshHit;
                        if (NavMesh.SamplePosition(raycastHit.point, out navMeshHit, 1.0f, NavMesh.AllAreas))
                        {
                            float areaCost = NavMesh.GetAreaCost(navMeshHit.mask);
                            spawnPointsWithCosts.Add((t, areaCost));
                        }
                        else
                        {
                            Debug.LogWarning($"레이캐스트 위치 {raycastHit.point} 근처에서 네비메쉬를 찾을 수 없습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"스폰 포인트 위치 {t.position} 아래로 레이캐스트가 실패했습니다.");
                    }
                }
            }

            // 2. 가중치에 따라 스폰 포인트를 내림차순으로 정렬
            spawnPointsWithCosts.Sort((a, b) => b.areaCost.CompareTo(a.areaCost));

            // 3. 정렬된 스폰 포인트에 유물 배치
            foreach (var (spawnPoint, areaCost) in spawnPointsWithCosts)
            {
                ProbabilityTable selectedProbabilityTable = probabilityTables[Random.Range(0, probabilityTables.Count)];
                Relices selectedRelicInfo = SelectRelicByProbabilityAndType(selectedProbabilityTable, GetRoomTypeForSpawnPoint(spawnPoint));

                if (selectedRelicInfo == null)
                {
                    Debug.LogWarning("유물 선택 실패 - 확률 테이블에서 선택된 유물이 없습니다.");
                    continue;
                }

                // 가중치에 따라 선택된 유물을 필터링
                if (IsRelicSuitableForArea(selectedRelicInfo, areaCost))
                {
                    var randomTempRelic = tempRelicPrefab[Random.Range(0, tempRelicPrefab.Length)];
                    var networkObjRelic = Runner.Spawn(randomTempRelic, spawnPoint.position);
                    var tempRelic = networkObjRelic.GetComponent<TempRelic>();

                    networkObjRelic.transform.SetParent(relicPool);

                    // 유물 정보 설정
                    tempRelic.GoldPoint = selectedRelicInfo.goldPoint;
                    tempRelic.RenownPoint = selectedRelicInfo.renownPoint;
                    tempRelic.RoomNum = GetRoomIndexForSpawnPoint(spawnPoint);
                    tempRelic.relicNumber = relicCount;
                    tempRelic.RelicType = (TempRelic.Type)selectedRelicInfo.type;
                    tempRelic.Excavation = selectedRelicInfo.excavation;
                    tempRelic.Damage = selectedRelicInfo.damage;

                    // 시각적 인덱스 설정 및 동기화
                    Transform visualTransform = tempRelic.visual.transform;
                    int chosenIndex = GetChosenVisualIndex(tempRelic, visualTransform.childCount);
                    tempRelic.ChosenVisualIndex = chosenIndex; // 네트워크를 통해 동기화

                    // 모든 플레이어에게 시각적 인덱스를 적용
                    for (int childIndex = 0; childIndex < visualTransform.childCount; childIndex++)
                    {
                        visualTransform.GetChild(childIndex).gameObject.SetActive(childIndex == chosenIndex);
                    }

                    SpawnedRelics.Set(relicCount, tempRelic);
                    relicCount++;
                }
                else
                {
                    Debug.LogWarning($"선택된 유물이 해당 영역의 가중치와 적합하지 않습니다. (Area Cost: {areaCost})");
                }
            }
        }

        private int GetRoomIndexForSpawnPoint(Transform spawnPoint)
        {
            for (int i = 0; i < tempRooms.Length; i++)
            {
                if (tempRooms[i].tempRelicSpawnPoints.Contains(spawnPoint))
                {
                    return i;
                }
            }
            return -1; // 스폰 포인트가 어떤 방에도 속하지 않는 경우 -1 반환
        }

        private int GetRoomTypeForSpawnPoint(Transform spawnPoint)
        {
            for (int i = 0; i < tempRooms.Length; i++)
            {
                if (tempRooms[i].tempRelicSpawnPoints.Contains(spawnPoint))
                {
                    return tempRooms[i].roomtype;
                }
            }
            return 0; // 기본 방 타입 반환
        }
        private bool IsRelicSuitableForArea(Relices relic, float areaCost)
        {
            // 영역 가중치(areaCost)에 따른 유물의 적합성 결정 로직
            int targetDepth = 3; // 시작하는 목표 깊이, HighValue에 해당하는 최상위 깊이

            if (areaCost == 7.0f) // HighValue 영역
            {
                targetDepth = 3;
            }
            else if (areaCost == 6.0f) // MidValue 영역
            {
                targetDepth = 2;
            }
            else if (areaCost == 5.0f) // LowValue 영역
            {
                targetDepth = 1;
            }
            else if (areaCost == 1.0f) // Walkable 영역
            {
                targetDepth = 0;
            }

            // 주어진 영역에 맞는 깊이를 찾기 위한 루프
            while (targetDepth >= 0)
            {
                if (relic.depth >= targetDepth)
                {
                    return true;
                }

                targetDepth--; // 목표 깊이를 하나 낮춰서 다시 검사
            }

            return false;
        }
        // 시각적 인덱스를 결정하는 로직을 별도의 메서드로 분리 ->7.18
        private int GetChosenVisualIndex(TempRelic relic, int childCount)
        {
            int startIndex = 0;
            int endIndex = childCount;

            if (relic.RelicType == TempRelic.Type.GoldRelic)
            {
                startIndex = 0;
                endIndex = 3;
            }
            else if (relic.RelicType == TempRelic.Type.RenownRelic)
            {
                startIndex = 3;
                endIndex = 6;
            }
            return Random.Range(startIndex, endIndex);
        }

        private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)
        {
            if (table.probabilities == null || table.probabilities.Count == 0)
            {
                Debug.LogWarning("확률 테이블이 비어 있습니다.");
                return null;
            }

            int randomPoint = UnityEngine.Random.Range(0, 100);
            int cumulativeProbability = 0;

            foreach (YearProbability yearProb in table.probabilities)
            {
                cumulativeProbability += yearProb.probability;

                if (randomPoint < cumulativeProbability)
                {
                    List<Relices> filteredRelics = new List<Relices>();

                    foreach (var relic in relicess)
                    {
                        if (relic.depth == yearProb.relicdepth && IsMatchingType(relic, roomType))
                        {
                            filteredRelics.Add(relic);
                        }
                    }

                    if (filteredRelics.Count > 0)
                    {
                        return filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    }
                    else
                    {
                        Debug.LogWarning($"조건에 맞는 유물이 없습니다. relicdepth: {yearProb.relicdepth}, roomType: {roomType}");
                    }
                    break;
                }
            }

            Debug.LogWarning("유물 선택에 실패했습니다. 확률 범위에 해당하는 유물이 없습니다.");
            return null;
        }

        private bool IsMatchingType(Relices relic, int roomType)//7.15 유물 생성 추가 중
        {
            return (roomType == 1 && relic.type == Relices.RelicesType.NormalRelic) ||
                   (roomType == 2 && relic.type == Relices.RelicesType.GoldRelic) ||
                   (roomType == 3 && relic.type == Relices.RelicesType.RenownRelic);
        }
        public TempRelic GetTempRelicWithIndex(int index)
        {
            var networkObject = SpawnedRelics.Get(index);
            return networkObject;
        }

        void ReadCSV()//7.15 유물 생성 추가 중
        {
            string[] rows = relicPrefabsData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < rows.Length; i++) // 헤더를 제외하고 시작
            {
                string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
                Relices relic = new Relices();
                if (Enum.TryParse(data[0], out Relices.RelicesType type))
                {
                    relic.type = type;
                }
                else
                {
                    Debug.LogError($"Invalid Type on line {i + 1}");
                    continue;
                }

                if (!int.TryParse(data[1], out int goldPoint) ||
                    !int.TryParse(data[2], out int renownPoint) ||
                    !int.TryParse(data[3], out int depth) ||
                    !int.TryParse(data[4], out int RoomNum) ||
                    !int.TryParse(data[5], out int excavation) ||//->9.25
                    !int.TryParse(data[6], out int damage))//->9.25
                {
                    Debug.LogError($"Data format error on line {i + 1}");
                    continue;
                }

                relic.depth = depth;
                relic.goldPoint = goldPoint;
                relic.renownPoint = renownPoint;
                relic.RoomNum = RoomNum;
                relic.excavation = excavation; //->9.25
                relic.damage = damage; //->9.25

                relicess.Add(relic); // 직접 리스트에 추가
            }
        }

        void ReadProbabilityCSV()//7.15 유물 생성 추가 중
        {
            string[] rows = probabilityTableData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 1; i < rows.Length; i++)
            {
                string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
                ProbabilityTable table = new ProbabilityTable();
                table.probabilities = new List<YearProbability>
            {
                new YearProbability { relicdepth = 1, probability = int.Parse(data[1]) },
                new YearProbability { relicdepth = 2, probability = int.Parse(data[2]) },
                new YearProbability { relicdepth = 3, probability = int.Parse(data[3]) },
                new YearProbability { relicdepth = 4, probability = int.Parse(data[4]) }
            };
                probabilityTables.Add(table);
            }
        }
    }
}
