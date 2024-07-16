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
    public int relictier;
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
    public TextAsset relicPrefabsData; // 유물 CSV 파일 데이터
    public TextAsset probabilityTableData; // 확률 테이블 CSV 파일 데이터
    public List<Relices> relicess = new List<Relices>(); // 직접 유물 리스트 객체
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // 확률 테이블 리스트
    public GameObject[] RelicPrefabs; // 유물 프리팹 배열
    public List<Relic> createdRelicList;
   

    [System.Serializable]
    public class Relices
    {
        public RelicesType type; // 유물 타입
        public int tier; // 티어
        public int goldPoint; // 골드 포인트
        public int renownPoint; // 명성 포인트
        public int roomID; // 방 ID


        public enum RelicesType { NormalRelic, GoldRelic, RenownRelic } // 유물 타입 열거형
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

    void ReadProbabilityCSV()
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

        int id = 0;
        foreach (Vector3 pos in positions)
        {
            Relices relicData = SelectRelicByProbabilityAndType(table, roomType);
            if (relicData != null)
            {
                int index = relicess.IndexOf(relicData); // 유물 정보 리스트에서 인덱스 찾기
                GameObject relicPrefab = RelicPrefabs[index]; // 같은 인덱스의 프리팹 참조
                GameObject instantiatedRelic = Instantiate(relicPrefab, pos, Quaternion.identity);
                instantiatedRelic.transform.SetParent(this.transform);

                Relic relicComponent = instantiatedRelic.GetComponent<Relic>();
                if (relicComponent == null)
                {
                    relicComponent = instantiatedRelic.AddComponent<Relic>();
                }
                relicComponent.ID = id++; // 임시 추가
                relicComponent.roomID = roomID;
                relicComponent.tier = relicData.tier;
                relicComponent.goldPoint = relicData.goldPoint;
                relicComponent.renownPoint = relicData.renownPoint;
                relicComponent.type = (Relic.Type)relicData.type;
                
                createdRelicList.Add(relicComponent);
                // 디버그 메시지 추가
                Debug.Log("유물 생성됨: " + instantiatedRelic.name + " 위치: " + pos + " RoomID: " + roomID);
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType) // 확률표에 따른 유물 선택
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<Relices> filteredRelics = new List<Relices>();

                // 유물 타입 필터링을 직접 처리
                foreach (var relic in relicess)
                {
                    if (relic.tier == yearProb.relictier)
                    {
                        bool isMatchingType = (roomType == 1 && relic.type == Relices.RelicesType.NormalRelic) ||
                                              (roomType == 2 && relic.type == Relices.RelicesType.GoldRelic) ||
                                              (roomType == 3 && relic.type == Relices.RelicesType.RenownRelic);
                        if (isMatchingType)
                        {
                            filteredRelics.Add(relic);
                        }
                    }
                }

                Debug.Log("필터링된 유물 개수: " + filteredRelics.Count + " (티어: " + yearProb.relictier + ", 타입: " + roomType + ")");

                // 필터링된 유물 중에서 무작위로 하나 선택
                if (filteredRelics.Count > 0)
                {
                    Relices selectedRelic = filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    return selectedRelic;
                }

                break;
            }
        }

        return null; // 적합한 유물을 찾지 못한 경우
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
