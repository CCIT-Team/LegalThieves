using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour //유물이 가지는 값
{
    public enum Type { Relics, goldRelics, renowonRelics }; //유물의 종류
    public Type type;
    public int year;   // 유물이 발견된 년도
    public int AllPoint; // 유물의 총 포인트
    public int GlodPoint;  // 유물의 가격
    public int RenownPoint;  // 유물의 명성
    public int RoomID;    // 방의ID
}
