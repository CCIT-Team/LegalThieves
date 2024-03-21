using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator2 : MonoBehaviour
{
    [SerializeField]
    private GameObject LobbyPrefabs;

    [SerializeField]
    private List<Room2> Room2Prefabs;

    [SerializeField]
    private Vector3 dungeonSize = new Vector3(0,0,0);
    [SerializeField]
    private bool multyFloor = false;

    [SerializeField]
    private int roomCountMax;
    [SerializeField]
    private int roomCountMin;

    List<Room> roomObjects;

    [SerializeField]
    List<Vector3Int> roomsizes;
    void Start()
    {
        CheckFloor();
        CreateDungen();
    }

    void SetRoomSize()
    {
        foreach(Room2 room2 in Room2Prefabs)
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
        int i = 0;
        CreateLobby();
        while (i < 10/*roomObjects.Count < roomCountMax*/)
        {
            CreateRoom();
            i++;
        }
    }

    void CreateLobby()
    {

    }

    void CreateRoom()
    {
        Instantiate(Room2Prefabs[Random.Range(0, Room2Prefabs.Count)]);
    }
}

public enum ROOMTYPE { None = -1, Lobby, Chamber, Corridor }
