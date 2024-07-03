using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class room : MonoBehaviour
{
    public int RoomID;
    public int RoomType;
    public int RoomValue;
    public List<Vector3> RelicsPositions;
    public Artifact Relics;
    // Start is called before the first frame update
    void Start()
    {
        RoomID = Random.Range(1, 13);
        RoomType = Random.Range(1, 4);
        RoomValue = Random.Range(1, 7);
        RelicsPositions = GenerateRelicsPositions();

        if (Relics != null)
        {
            Relics.PlaceRelics(RelicsPositions, RoomID, RoomValue, RoomType);
        }
        else
        {
            Debug.LogError("Relics manager not set");
        }
    }

    List<Vector3> GenerateRelicsPositions()
    {
        int count = Random.Range(3, 11);
        List<Vector3> positions = new List<Vector3>();

        for(int i = 0; i < count; i++)
        {
            float x = Random.Range(-10.0f, 10.0f);
            float z = Random.Range(-10.0f, 10.0f);
            positions.Add(new Vector3(x, 1, z));
        }
        return positions;
    }
}
