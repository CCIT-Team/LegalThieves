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
    public TextAsset textAssetData; // ���� CSV ���� ������
    public TextAsset probabilityTableData; // Ȯ�� ���̺� CSV ���� ������
    public RelicList myRelicList = new RelicList(); // ���� ����Ʈ ��ü
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // Ȯ�� ���̺� ����Ʈ
    public GameObject[] RelicPrefabs; // ���� ������ �迭

    [System.Serializable]
    public class RelicList
    {
        public Relic[] relics; // ���� �迭
    }

    void Start()
    {
        ReadCSV(); // CSV ���� �б�
        ReadProbabilityCSV(); // Ȯ�� ���̺� CSV ���� �б�
        Debug.Log("CSV ������ �ε� �Ϸ�: " + myRelicList.relics.Length + "���� ���� �ε��");

      
    }

    void ReadCSV()
    {
        string[] rows = textAssetData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int tableSize = rows.Length - 1; // ù ���� ����̹Ƿ� ����
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
            myRelicList.relics[i - 1].name = data[0]; // ������ �̸��� Name �ʵ�� ����
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

    public void PlaceRelic(List<Vector3> positions, int roomID, int roomValue, int roomType) //������ġ
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

                    // ����� �޽��� �߰��Ͽ� ������ �ùٸ��� �����Ǿ����� Ȯ��
                    Debug.Log("���� ������: " + instantiatedRelic.name + " ��ġ: " + pos + " RoomID: " + roomID);
                }
                else
                {
                    Debug.LogError("�������� �ε��� �� �����ϴ�: " + relicData.name);
                }
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relic SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)// Ȯ��ǥ�� ���� ���� ����
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
                    Debug.Log("���õ� ����: " + selectedRelic.name);
                    return selectedRelic;
                }
                break;
            }
        }
        return null;
    }
    private List<Relic> FilterRelicByyearAndType(int minyear, int maxyear, int roomType)// ���� Ÿ�� ����
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

        // ����� �޽��� �߰��Ͽ� ���͸��� ���� ����Ʈ Ȯ��
        Debug.Log("���͸��� ���� ����: " + filteredRelics.Count + " (�⵵: " + minyear + "-" + maxyear + ", Ÿ��: " + roomType + ")");
        return filteredRelics;
    }

    // �������� �̸����κ��� �ε��ϴ� �Լ�
    private GameObject LoadPrefab(string prefabName)
    {
        foreach (var prefab in RelicPrefabs)
        {
            if (prefab.name == prefabName)
            {
                return prefab;
            }
        }
        Debug.LogError("�������� �ε��� �� �����ϴ�: " + prefabName);
        return null;
    }
}