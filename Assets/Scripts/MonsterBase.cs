using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : MonoBehaviour
{
    protected Camera _camera; // 메인 카메라
    protected Transform player;
    enum MosterType { zombie, mira, ghost            , Boss0 } //몬스터,  보스 타입 지정
    [SerializeField] MosterType monsterType;
    [SerializeField] bool isTrigger ;

    Dictionary<string, float> SpeedDictionary = new()
    {
        // 몬스터들의 속도 {"이름",속도}
        {"zombie",5},
        {"mummy", 2},
        {"ghost", 1},

        //보스몬스터 속도
        {"Boss0", 40}
     
    };

    
    public NavMeshAgent agent;

    
    private void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
      

    }
    // Start is called before the first frame update
    
    public void Init() //enum 값에 따라 초기화하는 함수
    {
        switch (monsterType)
        {
            case MosterType.zombie:
               // 딕셔너리에 정의된 속도를 가져옴
                agent.speed = SpeedDictionary["zombie"];
                break;

            case MosterType.mira:
               
                agent.speed = SpeedDictionary["mummy"];
                break;

            case MosterType.ghost:
      
                agent.speed = SpeedDictionary["ghost"];
                break;
            case MosterType.Boss0:
                _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                agent.speed = SpeedDictionary["Boss0"];
                break;
        }
    }

}
