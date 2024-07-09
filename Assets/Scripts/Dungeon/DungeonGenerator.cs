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

    public void GenerateDungeon()
    {
        #region 랜덤 방 생성_삭제예정
        if (isCreated)
            return;
        isCreated = true;
        Vector3 roomPos = Vector3.zero;
        for (int i = 0; i < roomCount; i++)
        {
            roomPos = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
            Room room = Instantiate(roomPrefab, roomPos, Quaternion.identity).GetComponent<Room>();
            room.roomID = i;
            Rooms.Add(room);
        }
        #endregion
    }
}
