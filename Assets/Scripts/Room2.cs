using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room2 : MonoBehaviour
{
    [Header("Parameter")]
    [Range(1, 4)]
    public int maxDoorCount = 1;
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

    public void ActiveDoor(int i)
    {
        if(i >= doors.Count)
        {
            return;
        }
        walls[i].SetActive(false);
        doors[i].SetActive(true); 
    }
}
