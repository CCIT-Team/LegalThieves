using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Relic : MonoBehaviour //������ ������ ��
{
    //ID�� ��� �ӽ� �߰�
    public int ID;
    public enum Type { NormalRelic, GoldRelic, RenownRelic }; //������ ����
    public Type type;
    public int tier;   // ������ Ƽ��
    public int goldPoint;  // ������ ����
    public int renownPoint;  // ������ ��
    public int roomID;    // ����ID
}
