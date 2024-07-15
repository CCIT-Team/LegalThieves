<<<<<<< HEAD
using System;
using System.Collections.Generic;
=======
using System.Linq;
>>>>>>> Player_Interaction
using Fusion;
using UnityEngine;
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

        [Space]
        [Header("Setup")]
        [SerializeField] private float depthMuliple = 1f;

        //스폰된 유물이 담기는 네트워크배열. 네트워크배열에 관해서는 포톤 doc 참고.
        [Networked, Capacity(200)] private NetworkArray<TempRelic> SpawnedRelics { get; }

        [System.Serializable]//7.15일 유물생성 추가 중
        public struct YearProbability
        {
            public int relictier;
            public int probability;
        }

        [System.Serializable]//7.15일 유물생성 추가 중
        public class ProbabilityTable
        {
            public List<YearProbability> probabilities;
        }

        [System.Serializable]//7.15일 유물생성 추가 중
        public class Relices
        {
            public RelicesType type; // 유물 타입
            public int tier; // 티어
            public int goldPoint; // 골드 포인트
            public int renownPoint; // 명성 포인트
            public int roomID; // 방 ID


            public enum RelicesType { NormalRelic, GoldRelic, RenownRelic } // 유물 타입 열거형
        }

        [System.Serializable]//7.15일 유물생성 추가 중
        public class RelicesList
        {
            public Relices[] Relicess; // csv파일읽고 생성된 유물 배열
        }

        public int roomID; // 방 ID 7.15일 유물생성 추가 중
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

<<<<<<< HEAD
        private void SpawnRelics()//7.15 유물 생성 추가 중
=======
        //모든 유물을 스폰하는 함수. 이후에 게임 state 구현되면 게임 준비 혹은 게임 시작 state에 호출되어 유물을 생성할 예정.
        //지금은 게임이 시작되면(호스트가 접속하면) 생성됨.
        //라운드 개념이 구현될 때 생성된 유물을 전부 디스폰하고 리스트를 초기화하는 작업이 구현될 필요가 있어보임.
        private void SpawnRelics()
>>>>>>> Player_Interaction
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
<<<<<<< HEAD

                    // 유물 정보 설정
                    var selectedRelicInfo = SelectRelicByProbabilityAndType(probabilityTables[Random.Range(0, probabilityTables.Count)], tempRooms[i].roomtype);// 추가한 부분
                    if (selectedRelicInfo != null)// 추가한 부분
                    {
                        // TempRelic 스크립트의 속성을 설정
                        tempRelic.GoldPoint = selectedRelicInfo.goldPoint;// 추가한 부분
                        tempRelic.RenownPoint = selectedRelicInfo.renownPoint;// 추가한 부분
                        tempRelic.RoomNum = i;
                        tempRelic.relicNumber = relicCount;
                        tempRelic.type = (TempRelic.Type)selectedRelicInfo.type;
                    }
=======
                    
                    tempRelic.relicNumber = relicCount;
                    tempRelic.RoomNum = i;


>>>>>>> Player_Interaction

                    SpawnedRelics.Set(relicCount, tempRelic);
                    relicCount++;
                }
            }
        }

<<<<<<< HEAD
        private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)//7.15 유물 생성 추가 중
        {
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
                        if (relic.tier == yearProb.relictier && IsMatchingType(relic, roomType))
                        {
                            filteredRelics.Add(relic);
                        }
                    }

                    if (filteredRelics.Count > 0)
                    {
                        return filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    }
                    break;
                }
            }
            return null;
        }

        private bool IsMatchingType(Relices relic, int roomType)//7.15 유물 생성 추가 중
        {
            return (roomType == 1 && relic.type == Relices.RelicesType.NormalRelic) ||
                   (roomType == 2 && relic.type == Relices.RelicesType.GoldRelic) ||
                   (roomType == 3 && relic.type == Relices.RelicesType.RenownRelic);
        }
=======
        //
>>>>>>> Player_Interaction
        public TempRelic GetTempRelicWithIndex(int index)
        {
            var tempRelic = SpawnedRelics[index];
            return tempRelic == null ? null : tempRelic;
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
                    !int.TryParse(data[3], out int tier) ||
                    !int.TryParse(data[4], out int roomID))
                {
                    Debug.LogError($"Data format error on line {i + 1}");
                    continue;
                }

                relic.tier = tier;
                relic.goldPoint = goldPoint;
                relic.renownPoint = renownPoint;
                relic.roomID = roomID;

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
                new YearProbability { relictier = 1, probability = int.Parse(data[1]) },
                new YearProbability { relictier = 2, probability = int.Parse(data[2]) },
                new YearProbability { relictier = 3, probability = int.Parse(data[3]) },
                new YearProbability { relictier = 4, probability = int.Parse(data[4]) }
            };
                probabilityTables.Add(table);
            }
        }
    }
}
