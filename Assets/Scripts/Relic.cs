using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relic : MonoBehaviour //유물이 가지는 값
{
    public enum Type { Relic, GoldRelic, RenownRelic }; //유물의 종류
    public Type type;
    public int year;   // 유물이 발견된 년도
    public int AllPoint; // 유물의 총 포인트
    public int GoldPoint;  // 유물의 가격
    public int RenownPoint;  // 유물의 명성
    public int RoomID;    // 방의ID
}
