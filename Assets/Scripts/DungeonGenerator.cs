using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    /*
    0 최초 노드 생성
    1 노드 연결 가능 통로 탐색
    2 전혀 없을 경우 0으로 리셋
    3 연결 노드 생성
    4 새 노드에서 1 실행
    5 전혀 없을 경우 이전 노드로 귀환
    6 방문하지 않은 통로 탐색
    7 모든 노드 생성시까지 3 ~ 6 반복
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
