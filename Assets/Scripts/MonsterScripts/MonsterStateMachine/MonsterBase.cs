using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBase : NetworkBehaviour
{
    //protected Camera _camera; // 메인 카메라
    protected Transform Near;
       [Networked] public Vector3 Position { get; set; } // 몬스터의 위치 동기화
    [Networked] public Vector3 Rotation {  get; set; }
    enum MosterType { zombie, mira, ghost            , Boss0 } //몬스터,  보스 타입 지정
    [SerializeField] MosterType monsterType;
    public List<Transform> patrolPoints = new List<Transform>();

    [Networked] public Vector3 agentVelocity { get { return _agent.velocity; } set { } }

    [Networked] public float agentRemainingDistance { get { return _agent.remainingDistance; } set { } }
  
    private NavMeshAgent _agent;


    Dictionary<string, float> SpeedDictionary = new()
    {
        // 몬스터들의 속도 {"이름",속도}
        {"zombie",5},
        {"mummy", 2},
        {"ghost", 1},

        //보스몬스터 속도
        {"Boss0", 40}
     
    };


    private void Update()
    {
        if (Object.HasStateAuthority)
        {
            Position = _agent.nextPosition;
        }
    }
    // Start is called before the first frame update

    protected void Init() //enum 값에 따라 초기화하는 함수
    {
        _agent = gameObject.GetComponent<NavMeshAgent>();

        switch (monsterType)
        {
            case MosterType.zombie:
                // 딕셔너리에 정의된 속도를 가져옴
                _agent.speed = 5;
                break;
                
            case MosterType.mira:

                _agent.speed = SpeedDictionary["mummy"];
                break;

            case MosterType.ghost:

                _agent.speed = SpeedDictionary["ghost"];
                break;
            //case MosterType.Boss0:
            //    _camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            //    agentSpeed = SpeedDictionary["Boss0"];
            //    break;
        }
    }
    public Transform CheckNearPlayer()
    {
        NetworkObject[] players = new NetworkObject[PlayerManager.Instance.PlayerList.Length];

        for (int i = 0; i < PlayerManager.Instance.PlayerList.Length; i++)
        {
            players[i] = PlayerManager.Instance.PlayerList.Get(i); // NetworkArray의 데이터를 일반 배열로 복사
        }

        NetworkObject near = players[0];

        for (int i = 0; i < players.Length; i++)
        {
            if (Vector3.Distance(transform.position, near.transform.position) >
                Vector3.Distance(transform.position, players[i].transform.position))
            {
                near = players[i];
            }
        }
        return near.transform;
    }

  
    protected void MoveTo(Vector3 newPosition)
    {
        if (Object.HasStateAuthority)
        {
            _agent.SetDestination(newPosition);
        }
    }
}
