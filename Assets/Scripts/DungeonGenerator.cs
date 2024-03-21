using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    List<Room> rooms = new List<Room>();
    List<Room> ways = new List<Room>();
    public GameObject door;

    [SerializeField]
    Vector3Int maxDungeonSize;
    [SerializeField]
    Vector3Int maxRoomSize;

    [SerializeField]
    List<GameObject> roomPrefabs;
    List<GameObject> roomObjects = new List<GameObject>();

    bool isGenerated;
    private 
    void Start()
    {
        if(!isGenerated)
        {
            StartGenerate();
        }
    }

    public void StartGenerate()
    {
        int test = 0;
        while(roomObjects.Count < 8)
        {
            CreateRoom();
            test++;
            if(test >= 200)
            {
                Debug.Log("over 200");
                break;
            }
        }
    }

    void CreateRoom()
    {
        Vector3Int roomPosition = new Vector3Int(Random.Range(0, maxDungeonSize.x), 0, Random.Range(0, maxDungeonSize.z));
        Vector3Int roomSize = new Vector3Int(Random.Range(1, maxRoomSize.x), /*Random.Range(1, maxRoomSize.y)*/5, Random.Range(1, maxRoomSize.z)) / 5 * 5 + new Vector3Int(5,5,5);
        Room currentRoom = new Room(roomPosition, roomSize);
        foreach(Room room in rooms)
        {
            if(!(room.bounds.max.x < currentRoom.bounds.min.x || room.bounds.max.y < currentRoom.bounds.min.y || room.bounds.max.z < currentRoom.bounds.min.z ||
               currentRoom.bounds.max.x < room.bounds.min.x || currentRoom.bounds.max.y < room.bounds.min.y || currentRoom.bounds.max.z < room.bounds.min.z ))
            {
                return;
            }
        }
        rooms.Add(currentRoom);
        currentRoom.setDoors();
        GameObject roomObject = Instantiate(roomPrefabs[0], currentRoom.bounds.center, Quaternion.identity, this.transform);
        roomObject.transform.localScale = roomSize;
        roomObjects.Add(roomObject);
    }

    void CreateWay()
    {
        foreach (Room room in rooms)
        {
            //room.doors[Random.Range(0,room.doors.Count)]
        }
    }
}
