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
public class RelicCreation : MonoBehaviour
{

    public GameObject[] RelicPrefabs;
    public int RoomID;
    public List<ProbabilityTable> probabilityTables; 
    
    public void PlaceRelic(List<Vector3> positions, int roomID, int roomValue, int roomType) // 유물 배치
    {
        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("No positions provided to place Relics.");
            return;
        }

        ProbabilityTable table = probabilityTables[roomValue -1];

        foreach (Vector3 pos in positions)
        {
            GameObject Relic = SelectRelicByProbabilityAndType(table, roomType);
            if (Relic != null)
            {
                GameObject instantiatedRelic = Instantiate(Relic, pos, Quaternion.identity);
                instantiatedRelic.transform.SetParent(this.transform);

                Relic RelicComponent = instantiatedRelic.GetComponent<Relic>();
                if (RelicComponent == null)
                {
                    RelicComponent = instantiatedRelic.AddComponent<Relic>();
                }
                RelicComponent.RoomID = roomID;
            }
            else
            {
                Debug.LogWarning("No suitable Relics found based on the probability table and room type.");
            }
        }
    }

    private GameObject SelectRelicByProbabilityAndType(ProbabilityTable table, int roomType) // 확률표에 따른 유물 선택
    {
        int randomPoint = Random.Range(0, 100);
        int cumulativeProbability = 0;

        foreach (YearProbability yearProb in table.probabilities)
        {
            cumulativeProbability += yearProb.probability;
            if (randomPoint < cumulativeProbability)
            {
                List<GameObject> filteredRelic = FilterRelicByYearAndType(yearProb.minYear, yearProb.maxYear, roomType);
                if (filteredRelic.Count > 0)
                {
                    return filteredRelic[Random.Range(0, filteredRelic.Count)];
                }
                break;
            }
        }
        return null;
    }

    private List<GameObject> FilterRelicByYearAndType(int minYear, int maxYear, int roomType) //방의 타입에 따른 필터 
    {
        List<GameObject> filteredRelic = new List<GameObject>();
        foreach (var relicPrefab in RelicPrefabs) 
        {
            Relic relicComponent = relicPrefab.GetComponent<Relic>(); 
            if (relicComponent != null && relicComponent.year >= minYear && relicComponent.year <= maxYear)
            {
                if (roomType == 1 ||
                    (roomType == 2 && relicComponent.type == Relic.Type.goldRelic) ||
                    (roomType == 3 && relicComponent.type == Relic.Type.renowonRelic))
                {
                    filteredRelic.Add(relicPrefab); 
                }
            }
        }
        return filteredRelic;
    }
}