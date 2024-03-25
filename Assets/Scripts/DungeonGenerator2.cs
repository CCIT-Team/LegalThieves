using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator2 : MonoBehaviour
{
    [SerializeField]
    private List<Room2> LobbyPrefabs;

    [SerializeField]
    private List<Room2> ChamberPrefabs;

    [SerializeField]
    private List<Room2> CorridorPrefabs;

    [SerializeField]
    private Vector3 dungeonSize = new Vector3(0,0,0);
    [SerializeField]
    private bool multyFloor = false;

    [SerializeField]
    private int roomCountMax;
    [SerializeField]
    private int roomCountMin;

    List<Room2> roomObjects = new List<Room2>();
    Room2 currentRoom = null;

    [SerializeField]
    List<Vector3Int> roomsizes;
    void Start()
    {
        CheckFloor();
        CreateDungen();
    }

    void SetRoomSize()
    {
        foreach(Room2 room2 in ChamberPrefabs)
        {
            roomsizes.Add(room2.bounds.max - room2.bounds.min);
        }
    }

    void CheckFloor()
    {
        if (!multyFloor)
            dungeonSize.y = 0;
    }

    void CreateDungen()
    {
        int corridorCount = 0;
        CreateLobby();
        while (roomObjects.Count < roomCountMax)
        {
            if(currentRoom == null)
            {
                CreateLobby();
            }
            else
            {
                switch (currentRoom.roomType)
                {
                    case ROOMTYPE.None:
                        break;
                    case ROOMTYPE.Corridor:
                        if (Random.Range(0, 2) == 0 || corridorCount >= 2)
                        {
                            CreateChamber();
                            corridorCount = 0;
                        }
                        else
                        {
                            CreateCorridor();
                            corridorCount++;
                        }
                        break;
                    default:
                        CreateCorridor();
                        break;
                }
            }
        }
    }

    void CreateLobby()
    {
        currentRoom = Instantiate(LobbyPrefabs[Random.Range(0, LobbyPrefabs.Count)]);
    }

    void CreateChamber()
    {
        currentRoom = Instantiate(ChamberPrefabs[Random.Range(0, ChamberPrefabs.Count)]);
        roomObjects.Add(currentRoom);
    }

    void CreateCorridor()
    {
        currentRoom = Instantiate(CorridorPrefabs[Random.Range(0, CorridorPrefabs.Count)]);
    }
}

public enum ROOMTYPE { None = -1, Lobby, Chamber, Corridor }
