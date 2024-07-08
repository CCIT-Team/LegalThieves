using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor;

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
    public int RoomID; // �� ID
    public TextAsset textAssetData; // ���� CSV ���� ������
    public TextAsset probabilityTableData; // Ȯ�� ���̺� CSV ���� ������
    public PlayerList myPlayerList = new PlayerList(); // �÷��̾� ����Ʈ ��ü
    public List<ProbabilityTable> probabilityTables = new List<ProbabilityTable>(); // Ȯ�� ���̺� ����Ʈ
    public GameObject[] RelicPrefabs; // ���� ������ �迭

    [System.Serializable]
    public class Player
    {
        public string Name; // �̸�
        public PlayerType Type; // ���� Ÿ��
        public int Year; // �⵵
        public int AllPoint; // ��ü ����Ʈ
        public int GoldPoint; // ��� ����Ʈ
        public int RenownPoint; // �� ����Ʈ
        public int RoomID; // �� ID
        public string PrefabName; // ������ �̸�

        public enum PlayerType { Relic, GoldRelic, RenownRelic } // ���� Ÿ�� ������
    }

    [System.Serializable]
    public class PlayerList
    {
        public Player[] players; // �÷��̾� �迭
    }

    void Start()
    {
        ReadCSV(); // CSV ���� �б�
        ReadProbabilityCSV(); // Ȯ�� ���̺� CSV ���� �б�
        Debug.Log("CSV ������ �ε� �Ϸ�: " + myPlayerList.players.Length + "���� �÷��̾� �ε��");

      
    }

    void ReadCSV()
    {
        string[] rows = textAssetData.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int tableSize = rows.Length - 1; // ù ���� ����̹Ƿ� ����
        myPlayerList.players = new Player[tableSize];

        for (int i = 1; i <= tableSize; i++)
        {
            string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);

            myPlayerList.players[i - 1] = new Player();
            myPlayerList.players[i - 1].Name = data[0];
            if (Enum.TryParse(data[1], out Player.PlayerType type))
            {
                myPlayerList.players[i - 1].Type = type;
            }
            else
            {
                Debug.LogError($"Invalid PlayerType on line {i + 1}");
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

            myPlayerList.players[i - 1].Year = year;
            myPlayerList.players[i - 1].AllPoint = allPoint;
            myPlayerList.players[i - 1].GoldPoint = goldPoint;
            myPlayerList.players[i - 1].RenownPoint = renownPoint;
            myPlayerList.players[i - 1].RoomID = roomID;
            myPlayerList.players[i - 1].PrefabName = data[0]; // ������ �̸��� Name �ʵ�� ����
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
            Player relicData = SelectRelicByProbabilityAndType(table, roomType);
            if (relicData != null)
            {
                GameObject relicPrefab = LoadPrefab(relicData.PrefabName);
                if (relicPrefab != null)
                {
                    GameObject instantiatedRelic = Instantiate(relicPrefab, pos, Quaternion.identity);
                    instantiatedRelic.transform.SetParent(this.transform);

                    Relic relicComponent = instantiatedRelic.GetComponent<Relic>();
                    if (relicComponent == null)
                    {
                        relicComponent = instantiatedRelic.AddComponent<Relic>();
                    }
                    relicComponent.RoomID = roomID;
                    relicComponent.year = relicData.Year;
                    relicComponent.AllPoint = relicData.AllPoint;
                    relicComponent.GoldPoint = relicData.GoldPoint;
                    relicComponent.RenownPoint = relicData.RenownPoint;
                    relicComponent.type = (Relic.Type)relicData.Type;

                    // ����� �޽��� �߰��Ͽ� ������ �ùٸ��� �����Ǿ����� Ȯ��
                    Debug.Log("���� ������: " + instantiatedRelic.name + " ��ġ: " + pos + " RoomID: " + roomID);
                }
                else
                {
                    Debug.LogError("�������� �ε��� �� �����ϴ�: " + relicData.PrefabName);
                }
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }
    private Player SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType)// Ȯ��ǥ�� ���� ���� ����
    {
        int randomPoint = UnityEngine.Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<Player> filteredRelics = FilterRelicByYearAndType(yearProb.minYear, yearProb.maxYear, roomType);
                if (filteredRelics.Count > 0)
                {
                    Player selectedRelic = filteredRelics[UnityEngine.Random.Range(0, filteredRelics.Count)];
                    Debug.Log("���õ� ����: " + selectedRelic.Name);
                    return selectedRelic;
                }
                break;
            }
        }
        return null;
    }
    private List<Player> FilterRelicByYearAndType(int minYear, int maxYear, int roomType)// ���� Ÿ�� ����
    {
        List<Player> filteredRelics = new List<Player>();

        foreach (var player in myPlayerList.players)
        {
            if (player.Year >= minYear && player.Year <= maxYear)
            {
                bool isMatchingType = (roomType == 1) ||
                                      (roomType == 2 && player.Type == Player.PlayerType.GoldRelic) ||
                                      (roomType == 3 && player.Type == Player.PlayerType.RenownRelic);
                if (isMatchingType)
                {
                    filteredRelics.Add(player);
                }
            }
        }

        // ����� �޽��� �߰��Ͽ� ���͸��� ���� ����Ʈ Ȯ��
        Debug.Log("���͸��� ���� ����: " + filteredRelics.Count + " (�⵵: " + minYear + "-" + maxYear + ", Ÿ��: " + roomType + ")");
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