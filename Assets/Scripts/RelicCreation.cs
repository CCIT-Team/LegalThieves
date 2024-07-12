using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

[System.Serializable]
public struct yearProbability
{
    public int minyear;
    public int maxyear;
    public int probability;
}

[System.Serializable]
public class ProbabilityTable
{
    public List<yearProbability> probabilities;
}

public class RelicCreation : MonoBehaviour
{
    public TextAsset textAssetData; // 유물 CSV 파일 데이터
    public TextAsset probabilityTableData; // 확률 테이블 CSV 파일 데이터
    public RelicList myRelicList = new RelicList(); // 유물 리스트 객체
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // 확률 테이블 리스트
    public GameObject[] RelicPrefabs; // 유물 프리팹 배열

    [System.Serializable]
    public class RelicList
    {
        public Relic[] relics; // 유물 배열
    }

    void Start()
    {
        ReadCSV(); // CSV 파일 읽기
        ReadProbabilityCSV(); // 확률 테이블 CSV 파일 읽기
        Debug.Log("CSV 데이터 로드 완료: " + myRelicList.relics.Length + "개의 유물 로드됨");

      
    }

    void ReadCSV()
    {
        string[] rows = textAssetData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int tableSize = rows.Length - 1; // 첫 줄은 헤더이므로 제외
        myRelicList.relics = new Relic[tableSize];

        for (int i = 1; i <= tableSize; i++)
        {
            string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);

            myRelicList.relics[i - 1] = new Relic();
            myRelicList.relics[i - 1].name = data[0];
            if (Enum.TryParse(data[1], out Relic.Type type))
            {
                myRelicList.relics[i - 1].type = type;
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

            myRelicList.relics[i - 1].year = year;
            myRelicList.relics[i - 1].allPoint = allPoint;
            myRelicList.relics[i - 1].goldPoint = goldPoint;
            myRelicList.relics[i - 1].renownPoint = renownPoint;
            myRelicList.relics[i - 1].roomID = roomID;
            myRelicList.relics[i - 1].name = data[0]; // 프리팹 이름은 Name 필드와 동일
        }
    }

    void ReadProbabilityCSV()
    {
        string[] rows = probabilityTableData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 1; i < rows.Length; i++)
        {
            string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
            ProbabilityTable table = new ProbabilityTable();
            table.probabilities = new List<yearProbability>
            {
                new yearProbability { minyear = 100, maxyear = 199, probability = int.Parse(data[1]) },
                new yearProbability { minyear = 200, maxyear = 399, probability = int.Parse(data[2]) },
                new yearProbability { minyear = 400, maxyear = 699, probability = int.Parse(data[3]) },
                new yearProbability { minyear = 700, maxyear = 799, probability = int.Parse(data[4]) },
                new yearProbability { minyear = 800, maxyear = 899, probability = int.Parse(data[5]) },
                new yearProbability { minyear = 900, maxyear = 999, probability = int.Parse(data[6]) }
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
            Relic relicData = SelectRelicByProbabilityAndType(table, roomType);
            if (relicData != null)
            {
                GameObject relicPrefab = LoadPrefab(relicData.name);
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
                    Debug.LogError("프리팹을 로드할 수 없습니다: " + relicData.name);
                }
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relic SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)// 확률표에 따른 유물 선택
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (yearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<Relic> filteredRelics = FilterRelicByyearAndType(yearProb.minyear, yearProb.maxyear, roomType);
                if (filteredRelics.Count > 0)
                {
                    Relic selectedRelic = filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    Debug.Log("선택된 유물: " + selectedRelic.name);
                    return selectedRelic;
                }
                break;
            }
        }
        return null;
    }
    private List<Relic> FilterRelicByyearAndType(int minyear, int maxyear, int roomType)// 유물 타입 필터
    {
        List<Relic> filteredRelics = new List<Relic>();

        foreach (var relic in myRelicList.relics)
        {
            if (relic.year >= minyear && relic.year <= maxyear)
            {
                bool isMatchingType = (roomType == 1) ||
                                      (roomType == 2 && relic.type == Relic.Type.GoldRelic) ||
                                      (roomType == 3 && relic.type == Relic.Type.RenownRelic);
                if (isMatchingType)
                {
                    filteredRelics.Add(relic);
                }
            }
        }

        // 디버그 메시지 추가하여 필터링된 유물 리스트 확인
        Debug.Log("필터링된 유물 개수: " + filteredRelics.Count + " (년도: " + minyear + "-" + maxyear + ", 타입: " + roomType + ")");
        return filteredRelics;
    }

    // 프리팹을 이름으로부터 로드하는 함수
    private GameObject LoadPrefab(string prefabName)
    {
        foreach (var prefab in RelicPrefabs)
        {
            if (prefab.name == prefabName)
            {
                return prefab;
            }
        }
        Debug.LogError("프리팹을 로드할 수 없습니다: " + prefabName);
        return null;
    }
}