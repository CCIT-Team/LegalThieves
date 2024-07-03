using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class Artifact : MonoBehaviour
{

    public GameObject[] RelicsPrefabs;
    public int RoomID;
    public List<ProbabilityTable> probabilityTables; 
    
    public void PlaceRelics(List<Vector3> positions, int roomID, int roomValue, int roomType) // 유물 배치
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("No positions provided to place Relics.");
            return;
        }

        ProbabilityTable table = probabilityTables[roomValue -1];

        foreach (Vector3 pos in positions)
        {
            GameObject Relics = SelectRelicsByProbabilityAndType(table, roomType);
            if (Relics != null)
            {
                GameObject instantiatedRelics = Instantiate(Relics, pos, Quaternion.identity);
                instantiatedRelics.transform.SetParent(this.transform);

                Item itemComponent = instantiatedRelics.GetComponent<Item>();
                if (itemComponent == null)
                {
                    itemComponent = instantiatedRelics.AddComponent<Item>();
                }
                itemComponent.RoomID = roomID;
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }

    private GameObject SelectRelicsByProbabilityAndType(ProbabilityTable table, int roomType) // 확률표에 따른 유물 선택
    {
        int randomPoint = Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<GameObject> filteredRelics = FilterRelicsByYearAndType(yearProb.minYear, yearProb.maxYear, roomType);
                if (filteredRelics.Count > 0)
                {
                    return filteredRelics[Random.Range(0, filteredRelics.Count)];
                }
                break;
            }
        }
        return null;
    }

    private List<GameObject> FilterRelicsByYearAndType(int minYear, int maxYear, int roomType) //방의 타입에 따른 필터 
    {
        List<GameObject> filteredRelics = new List<GameObject>();
        foreach (var Relics in RelicsPrefabs)
        {
            Item item = Relics.GetComponent<Item>();
            if (item != null && item.year >= minYear && item.year <= maxYear)
            {
                if (roomType == 1 ||
                    (roomType == 2 && item.type == Item.Type.goldRelics) ||
                    (roomType == 3 && item.type == Item.Type.renowonRelics))
                {
                    filteredRelics.Add(Relics);
                }
            }
        }
        return filteredRelics;
    }
}