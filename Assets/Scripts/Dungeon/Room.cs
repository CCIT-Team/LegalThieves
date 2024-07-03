using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ROOMTYPE { None = -1, NormalRoom, GoldRoom, RenownRoom }


public class Room : MonoBehaviour
{
    [SerializeField]
    List<GameObject> relicPositions;
    public int roomID = 0;
    [SerializeField]
    ROOMTYPE roomType = ROOMTYPE.NormalRoom;
    int roomValue = 0;


    public void SpawnRelics(GameObject relic)
    {
        for (int i = 0; i< 3; i++)
        {
            Instantiate(relic, relicPositions[Random.Range(0, relicPositions.Count)].transform.position, Quaternion.identity).GetComponent<DummyRelic>().roomID = roomID;
        }
    }
}
