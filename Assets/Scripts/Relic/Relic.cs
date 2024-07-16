using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relic : MonoBehaviour //유물이 가지는 값
{
    //ID가 없어서 임시 추가
    public int ID;
    public enum Type { NormalRelic, GoldRelic, RenownRelic }; //유물의 종류
    public Type type;
    public int tier;   // 유물의 티어
    public int goldPoint;  // 유물의 가격
    public int renownPoint;  // 유물의 명성
    public int roomID;    // 방의ID
}
