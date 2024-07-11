using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField]
    int roomCount = 0;
    [SerializeField]
    GameObject roomPrefab;

    List<Room> Rooms = new();
    bool isCreated = false;


    public void GenerateDungeon(Vector2 min2, Vector2 max2)
    {
        GenerateDungeon(min2.x, 0, min2.y, max2.x, 0, max2.y);
    }
    public void GenerateDungeon(Vector3 min3, Vector3 max3)
    {
        GenerateDungeon(min3.x,min3.y, min3.z, max3.x, max3.y, max3.z);
    }
    public void GenerateDungeon(float minX, float minY, float minZ, float maxX, float maxY, float maxZ)
    {
        #region 랜덤 방 생성_삭제예정
        if (isCreated)
            return;
        isCreated = true;
        Vector3 roomPos = Vector3.zero;
        for (int i = 0; i < roomCount; i++)
        {
            roomPos = new Vector3(Random.Range(minX, minZ), 0, Random.Range(maxX, maxZ));
            Room room = Instantiate(roomPrefab, roomPos, Quaternion.identity).GetComponent<Room>();
            room.roomID = i;
            Rooms.Add(room);
        }
        #endregion
    }
}
