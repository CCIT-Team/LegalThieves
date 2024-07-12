using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relic : MonoBehaviour //유물이 가지는 값
{
    public enum Type { Relic, GoldRelic, RenownRelic }; //유물의 종류
    public Type type;
    public int year;   // 유물이 발견된 년도
    public int allPoint; // 유물의 총 포인트
    public int goldPoint;  // 유물의 가격
    public int renownPoint;  // 유물의 명성
    public int roomID;    // 방의ID
}
