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
    public int roomID; // �� ID
    public TextAsset textAssetData; // ���� CSV ���� ������
    public TextAsset probabilityTableData; // Ȯ�� ���̺� CSV ���� ������
    public List<Relices> relicess = new List<Relices>(); // ���� ���� ����Ʈ ��ü
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // Ȯ�� ���̺� ����Ʈ
    public GameObject[] RelicPrefabs; // ���� ������ �迭

    [System.Serializable]
    public class Relices
    {
        public string name; // �̸�
        public RelicesType type; // ���� Ÿ��
        public int year; // �⵵
        public int allPoint; // ��ü ����Ʈ
        public int goldPoint; // ��� ����Ʈ
        public int renownPoint; // �� ����Ʈ
        public int roomID; // �� ID
        public string prefabname; // ������ �̸�


        public enum RelicesType { Relic, GoldRelic, RenownRelic } // ���� Ÿ�� ������
    }

    [System.Serializable]
    public class RelicesList
    {
        public Relices[] Relicess; // �÷��̾� �迭
    }

    void Start()
    {
        ReadCSV(); // CSV ���� �б�
        ReadProbabilityCSV(); // Ȯ�� ���̺� CSV ���� �б�
        Debug.Log("CSV ������ �ε� �Ϸ�: " + relicess.Count + "���� �÷��̾� �ε��");


    }

    void ReadCSV()
    {
        string[] rows = textAssetData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < rows.Length; i++) // ����� �����ϰ� ����
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
            relic.prefabname = data[0]; // ������ �̸��� name �ʵ�� ����

            relicess.Add(relic); // ���� ����Ʈ�� �߰�
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

                    // ����� �޽��� �߰��Ͽ� ������ �ùٸ��� �����Ǿ����� Ȯ��
                    Debug.Log("���� ������: " + instantiatedRelic.name + " ��ġ: " + pos + " RoomID: " + roomID);
                }
                else
                {
                    Debug.LogError("�������� �ε��� �� �����ϴ�: " + relicData.prefabname);
                }
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)// Ȯ��ǥ�� ���� ���� ����
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
                    Debug.Log("���õ� ����: " + selectedRelic.name);
                    return selectedRelic;
                }
                break;
            }
        }
        return null;
    }
    private List<Relices> FilterRelicByYearAndType(int minYear, int maxYear, int roomType)// ���� Ÿ�� ����
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

        // ����� �޽��� �߰��Ͽ� ���͸��� ���� ����Ʈ Ȯ��
        Debug.Log("���͸��� ���� ����: " + filteredRelics.Count + " (�⵵: " + minYear + "-" + maxYear + ", Ÿ��: " + roomType + ")");
        return filteredRelics;
    }

    // �������� �̸����κ��� �ε��ϴ� �Լ�
    private GameObject LoadPrefab(string prefabname)
    {
        foreach (var prefab in RelicPrefabs)
        {
            if (prefab.name == prefabname)
            {
                return prefab;
            }
        }
        Debug.LogError("�������� �ε��� �� �����ϴ�: " + prefabname + " / ������ �迭 ũ��: " + RelicPrefabs.Length);
        return null;
    }
}
