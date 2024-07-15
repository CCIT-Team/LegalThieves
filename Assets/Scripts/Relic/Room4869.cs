using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room4869 : MonoBehaviour
{
    public int roomID;
    public int roomType;
    public int roomValue;
    public List<Vector3> RelicPositions;
    public RelicCreation Relic;
    // Start is called before the first frame update
    void Start()
    {
        roomID = Random.Range(1, 13);
        roomType = Random.Range(1, 4);
        roomValue = Random.Range(1, 5);
        RelicPositions = GenerateRelicPositions();

        if (Relic != null)
        {
            Relic.PlaceRelic(RelicPositions, roomID, roomValue, roomType);
        }
        else
        {
            Debug.LogError("Relics manager not set");
        }
    }

    List<Vector3> GenerateRelicPositions()
    {
        int count = Random.Range(3, 11);
        List<Vector3> positions = new List<Vector3>();

        for (int i = 0; i < count; i++)
        {
            float x = Random.Range(-10.0f, 10.0f);
            float z = Random.Range(-10.0f, 10.0f);
            positions.Add(new Vector3(x, 1, z));
        }
        return positions;
    }
}
