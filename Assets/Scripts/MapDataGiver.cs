using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataGiver : MonoBehaviour
{
    public Vector3 maxRoomSize = new Vector3(10, 5, 10);
    public bool random = false;
    public List<Vector3> roomPositions;
    public List<Vector3> roomSizes;
    int roomcount = 0;
    public List<DummyRoom> rooms;
    void Start()
    {
        roomcount = Mathf.Min(roomPositions.Count, roomSizes.Count);
        if(random)
        {
            for (int i = 0; i < roomcount; i++)
            {
                rooms.Add(new DummyRoom(new Vector3(Random.Range(0,30), 0, Random.Range(0, 30)), new Vector3(Random.Range(0, maxRoomSize.x), 5, Random.Range(0, maxRoomSize.z))));
            }
        }
        else
        {
            for (int i = 0; i < roomcount; i++)
            {
                rooms.Add(new DummyRoom(roomPositions[i], roomSizes[i]));
            }
        }
    }
}

public class DummyRoom
{
    Vector2 roomposition;
    Vector2 roomsize;

    public DummyRoom(Vector3 position, Vector3 size)
    {
        roomposition = new Vector2(position.x, position.z);
        roomsize = new Vector2(size.x, size.z);
    }

    public Vector2 Position()
    {
        return roomposition;
    }

    public Vector2 Size()
    {
        return roomsize;
    }
}
