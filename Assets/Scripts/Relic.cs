using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relic : MonoBehaviour //������ ������ ��
{
    public enum Type { Relic, GoldRelic, RenownRelic }; //������ ����
    public Type type;
    public int year;   // ������ �߰ߵ� �⵵
    public int AllPoint; // ������ �� ����Ʈ
    public int GoldPoint;  // ������ ����
    public int RenownPoint;  // ������ ��
    public int RoomID;    // ����ID
}
