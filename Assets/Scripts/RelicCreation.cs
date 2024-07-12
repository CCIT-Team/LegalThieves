using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;
using static RelicCreation.Relices;
using static RelicCreation;

[System.Serializable]
public struct YearProbability
{
    public int minYear;
    public int maxYear;
    public int probability;
}

[System.Serializable]
public class ProbabilityTable
{
    public List<YearProbability> probabilities;
}

public class RelicCreation : MonoBehaviour
{
    public int roomID; // 방 ID
    public TextAsset textAssetData; // 유물 CSV 파일 데이터
    public TextAsset probabilityTableData; // 확률 테이블 CSV 파일 데이터
    public List<Relices> relicess = new List<Relices>(); // 직접 유물 리스트 객체
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // 확률 테이블 리스트
    public GameObject[] RelicPrefabs; // 유물 프리팹 배열

    [System.Serializable]
    public class Relices
    {
        public string name; // 이름
        public RelicesType type; // 유물 타입
        public int year; // 년도
        public int allPoint; // 전체 포인트
        public int goldPoint; // 골드 포인트
        public int renownPoint; // 명성 포인트
        public int roomID; // 방 ID
        public string prefabname; // 프리팹 이름


        public enum RelicesType { Relic, GoldRelic, RenownRelic } // 유물 타입 열거형
    }

    [System.Serializable]
    public class RelicesList
    {
        public Relices[] Relicess; // 플레이어 배열
    }

    void Start()
    {
        ReadCSV(); // CSV 파일 읽기
        ReadProbabilityCSV(); // 확률 테이블 CSV 파일 읽기
        Debug.Log("CSV 데이터 로드 완료: " + relicess.Count + "명의 플레이어 로드됨");


    }

    void ReadCSV()
    {
        string[] rows = textAssetData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < rows.Length; i++) // 헤더를 제외하고 시작
        {
            string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
            Relices relic = new Relices();
            relic.name = data[0];
            if (Enum.TryParse(data[1], out Relices.RelicesType type))
            {
                relic.type = type;
            }
            else
            {
                Debug.LogError($"Invalid Type on line {i + 1}");
                continue;
            }

            if (!int.TryParse(data[2], out int year) ||
                !int.TryParse(data[3], out int allPoint) ||
                !int.TryParse(data[4], out int goldPoint) ||
                !int.TryParse(data[5], out int renownPoint) ||
                !int.TryParse(data[6], out int roomID))
            {
                Debug.LogError($"Data format error on line {i + 1}");
                continue;
            }

            relic.year = year;
            relic.allPoint = allPoint;
            relic.goldPoint = goldPoint;
            relic.renownPoint = renownPoint;
            relic.roomID = roomID;
            relic.prefabname = data[0]; // 프리팹 이름은 name 필드와 동일

            relicess.Add(relic); // 직접 리스트에 추가
        }
    }

    void ReadProbabilityCSV()
    {
        string[] rows = probabilityTableData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < rows.Length; i++)
        {
            string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
            ProbabilityTable table = new ProbabilityTable();
            table.probabilities = new List<YearProbability>
            {
                new YearProbability { minYear = 100, maxYear = 199, probability = int.Parse(data[1]) },
                new YearProbability { minYear = 200, maxYear = 399, probability = int.Parse(data[2]) },
                new YearProbability { minYear = 400, maxYear = 699, probability = int.Parse(data[3]) },
                new YearProbability { minYear = 700, maxYear = 799, probability = int.Parse(data[4]) },
                new YearProbability { minYear = 800, maxYear = 899, probability = int.Parse(data[5]) },
                new YearProbability { minYear = 900, maxYear = 999, probability = int.Parse(data[6]) }
            };
            probabilityTables.Add(table);
        }
    }

    public void PlaceRelic(List<Vector3> positions, int roomID, int roomValue, int roomType) //유물배치
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("No positions provided to place Relics.");
            return;
        }

        if (roomValue < 1 || roomValue > probabilityTables.Count)
        {
            Debug.LogError("Invalid room value provided.");
            return;
        }

        ProbabilityTable table = probabilityTables[roomValue - 1];

        foreach (Vector3 pos in positions)
        {
            Relices relicData = SelectRelicByProbabilityAndType(table, roomType);
            if (relicData != null)
            {
                GameObject relicPrefab = LoadPrefab(relicData.prefabname);
                if (relicPrefab != null)
                {
                    GameObject instantiatedRelic = Instantiate(relicPrefab, pos, Quaternion.identity);
                    instantiatedRelic.transform.SetParent(this.transform);

                    Relic relicComponent = instantiatedRelic.GetComponent<Relic>();
                    if (relicComponent == null)
                    {
                        relicComponent = instantiatedRelic.AddComponent<Relic>();
                    }
                    relicComponent.roomID = roomID;
                    relicComponent.year = relicData.year;
                    relicComponent.allPoint = relicData.allPoint;
                    relicComponent.goldPoint = relicData.goldPoint;
                    relicComponent.renownPoint = relicData.renownPoint;
                    relicComponent.type = (Relic.Type)relicData.type;

                    // 디버그 메시지 추가하여 유물이 올바르게 생성되었는지 확인
                    Debug.Log("유물 생성됨: " + instantiatedRelic.name + " 위치: " + pos + " RoomID: " + roomID);
                }
                else
                {
                    Debug.LogError("프리팹을 로드할 수 없습니다: " + relicData.prefabname);
                }
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)// 확률표에 따른 유물 선택
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<Relices> filteredRelics = FilterRelicByYearAndType(yearProb.minYear, yearProb.maxYear, roomType);
                if (filteredRelics.Count > 0)
                {
                    Relices selectedRelic = filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    Debug.Log("선택된 유물: " + selectedRelic.name);
                    return selectedRelic;
                }
                break;
            }
        }
        return null;
    }
    private List<Relices> FilterRelicByYearAndType(int minYear, int maxYear, int roomType)// 유물 타입 필터
    {
        List<Relices> filteredRelics = new List<Relices>();

        foreach (var Relices in relicess)
        {
            if (Relices.year >= minYear && Relices.year <= maxYear)
            {
                bool isMatchingType = (roomType == 1) ||
                                      (roomType == 2 && Relices.type == Relices.RelicesType.GoldRelic) ||
                                      (roomType == 3 && Relices.type == Relices.RelicesType.RenownRelic);
                if (isMatchingType)
                {
                    filteredRelics.Add(Relices);
                }
            }
        }

        // 디버그 메시지 추가하여 필터링된 유물 리스트 확인
        Debug.Log("필터링된 유물 개수: " + filteredRelics.Count + " (년도: " + minYear + "-" + maxYear + ", 타입: " + roomType + ")");
        return filteredRelics;
    }

    // 프리팹을 이름으로부터 로드하는 함수
    private GameObject LoadPrefab(string prefabname)
    {
        foreach (var prefab in RelicPrefabs)
        {
            if (prefab.name == prefabname)
            {
                return prefab;
            }
        }
        Debug.LogError("프리팹을 로드할 수 없습니다: " + prefabname + " / 프리팹 배열 크기: " + RelicPrefabs.Length);
        return null;
    }
}
