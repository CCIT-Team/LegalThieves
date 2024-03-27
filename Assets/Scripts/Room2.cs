using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room2 : MonoBehaviour
{
    [Header("Parameter")]
    [Range(1, 4)]
    public int minDoorCount = 1;
    [Range(1, 4)]
    public int maxDoorCount = 1;
    int doorCount = 0;
    public ROOMTYPE roomType = ROOMTYPE.Chamber;
    [SerializeField]
    List<GameObject> walls = new List<GameObject>();
    [SerializeField]
    List<GameObject> doors = new List<GameObject>();

    [Header("DataView")]
    public BoundsInt bounds;
    public List<Transform> doorTransform;

    public Room2(Vector3Int location, Vector3Int size)
    {
        bounds = new BoundsInt(location, size);
    }

    public void SetDoor()
    {
        if (minDoorCount == maxDoorCount)
            doorCount = minDoorCount;
        else
            doorCount = Random.Range(minDoorCount, maxDoorCount);
        while (doorCount > 0 )
        {
            if(ActiveDoor(Random.Range(0, doors.Count)))
                doorCount--;
        }
    }

    public bool ActiveDoor(int i)
    {
        if(i >= doors.Count || doors[i].activeSelf)
        {
            return false;
        }
        walls[i].SetActive(false);
        doors[i].SetActive(true);
        return true;
    }
}
