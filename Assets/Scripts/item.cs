using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Item : MonoBehaviour //������ ������ ��
{
    public enum Type { Relics, goldRelics, renowonRelics }; //������ ����
    public Type type;
    public int year;   // ������ �߰ߵ� �⵵
    public int AllPoint; // ������ �� ����Ʈ
    public int GlodPoint;  // ������ ����
    public int RenownPoint;  // ������ ��
    public int RoomID;    // ����ID
}
