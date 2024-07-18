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
    public int roomID; // �� ID
    public TextAsset relicPrefabsData; // ���� CSV ���� ������
    public TextAsset probabilityTableData; // Ȯ�� ���̺� CSV ���� ������
    public List<Relices> relicess = new List<Relices>(); // ���� ���� ����Ʈ ��ü
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // Ȯ�� ���̺� ����Ʈ
    public GameObject[] RelicPrefabs; // ���� ������ �迭
    public List<Relic> createdRelicList;
   

    [System.Serializable]
    public class Relices
    {
        public RelicesType type; // ���� Ÿ��
        public int tier; // Ƽ��
        public int goldPoint; // ��� ����Ʈ
        public int renownPoint; // �� ����Ʈ
        public int roomID; // �� ID


        public enum RelicesType { NormalRelic, GoldRelic, RenownRelic } // ���� Ÿ�� ������
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
        string[] rows = relicPrefabsData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 1; i < rows.Length; i++) // ����� �����ϰ� ����
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
                new YearProbability { relictier = 1, probability = int.Parse(data[1]) },
                new YearProbability { relictier = 2, probability = int.Parse(data[2]) },
                new YearProbability { relictier = 3, probability = int.Parse(data[3]) },
                new YearProbability { relictier = 4, probability = int.Parse(data[4]) }
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

        int id = 0;
        foreach (Vector3 pos in positions)
        {
            Relices relicData = SelectRelicByProbabilityAndType(table, roomType);
            if (relicData != null)
            {
                int index = relicess.IndexOf(relicData); // ���� ���� ����Ʈ���� �ε��� ã��
                GameObject relicPrefab = RelicPrefabs[index]; // ���� �ε����� ������ ����
                GameObject instantiatedRelic = Instantiate(relicPrefab, pos, Quaternion.identity);
                instantiatedRelic.transform.SetParent(this.transform);

                Relic relicComponent = instantiatedRelic.GetComponent<Relic>();
                if (relicComponent == null)
                {
                    relicComponent = instantiatedRelic.AddComponent<Relic>();
                }
                relicComponent.ID = id++; // �ӽ� �߰�
                relicComponent.roomID = roomID;
                relicComponent.tier = relicData.tier;
                relicComponent.goldPoint = relicData.goldPoint;
                relicComponent.renownPoint = relicData.renownPoint;
                relicComponent.type = (Relic.Type)relicData.type;
                
                createdRelicList.Add(relicComponent);
                // ����� �޽��� �߰�
                Debug.Log("���� ������: " + instantiatedRelic.name + " ��ġ: " + pos + " RoomID: " + roomID);
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Relices SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType) // Ȯ��ǥ�� ���� ���� ����
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<Relices> filteredRelics = new List<Relices>();

                // ���� Ÿ�� ���͸��� ���� ó��
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

                Debug.Log("���͸��� ���� ����: " + filteredRelics.Count + " (Ƽ��: " + yearProb.relictier + ", Ÿ��: " + roomType + ")");

                // ���͸��� ���� �߿��� �������� �ϳ� ����
                if (filteredRelics.Count > 0)
                {
                    Relices selectedRelic = filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    return selectedRelic;
                }

                break;
            }
        }

        return null; // ������ ������ ã�� ���� ���
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
