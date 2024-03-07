using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    /*
    0 ���� ��� ����
    1 ��� ���� ���� ��� Ž��
    2 ���� ���� ��� 0���� ����
    3 ���� ��� ����
    4 �� ��忡�� 1 ����
    5 ���� ���� ��� ���� ���� ��ȯ
    6 �湮���� ���� ��� Ž��
    7 ��� ��� �����ñ��� 3 ~ 6 �ݺ�
    */

    [SerializeField]
    List<GameObject> Rooms;
    bool isGenerated = false;
    [SerializeField]
    private 
    void Start()
    {
        if(!isGenerated)
        {
            StartGenerate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGenerate()
    {
        //while(true)
        {
            CreateNode();
        }
    }

    void CreateNode()
    {
        Room node = Instantiate(Rooms[Random.Range(0, Rooms.Count)],this.transform).GetComponent<Room>();
        node.CheckRoom();
    }
}
