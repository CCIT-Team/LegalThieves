using Fusion;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class Monster : MonsterBase
{
 
    [Networked] public State MonsterState { get; set; } // 몬스터 상태 동기화
    [SerializeField]
  
    public enum State { IDLE, Chase, Patrol, Attack };



    //Patrol property
    private float timer;
    private int PatrolDelay = 2;

    //---------Patrol End

    //Chase property
    private Vector3 homePoint;
   

    //---------Chase End
    //Attack property
    private float attacktimer = 0;
    private int attackDelay = 1;

    //---------Attack End

    [SerializeField]
    private bool _StayMob;
    public bool StayMob { get { return _StayMob;} }


    private void Start()
    {
        if (HasStateAuthority)
        {
            switch (StayMob)
            {
                case true:
                    MonsterState = State.IDLE;
                    break;
                case false:
                    MonsterState = State.Patrol;
                    break;
            }
            // 서버만 몬스터를 제어하도록 설정
            Init();
        }
    }
   


    // 서버가 위치 업데이트를 처리하고, 이를 모든 클라이언트에 동기화

    private void Update()
    {
        // 상태에 따른 행동을 업데이트 (서버에서만 실행)
        if (HasStateAuthority)
        {
            
            switch (MonsterState)
            {
                case State.IDLE:
                    IDLE();
                    break;
                case State.Patrol:
                    Patrol();
                    break;
                case State.Chase:
                    Chase();
                    break;
                case State.Attack:
                    Attack();
                    break;
            }
          
        }

        // 클라이언트에서도 동기화된 위치를 적용
        transform.position = Position;
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority)
        {
            switch (MonsterState)
            {
                case State.IDLE:
                case State.Patrol:
                    Near = CheckNearPlayer();
                    homePoint = transform.position;
                    MonsterState = State.Chase;

                    break;

                case State.Attack:
                    break;

            }
        }
    
    }

    void IDLE()
    {
        //대기 애니메이션
    }


    //patrol 관련---------------------------------

    private void Patrol()
    {
        if (agentVelocity != Vector3.zero) return;
        Debug.Log("실행");
        timer += Time.deltaTime;

        if (timer > PatrolDelay)
        {
            timer = 0;
            NearPointSet(patrolPoints, 0, patrolPoints.Count - 1);
            MoveTo(patrolPoints[Random.Range(2, patrolPoints.Count-1)].position);
        }
    }

    private void NearPointSet(List<Transform> arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);
            NearPointSet(arr, low, pivotIndex - 1);
            NearPointSet(arr, pivotIndex + 1, high);
        }
    }

    private int Partition(List<Transform> arr, int low, int high)
    {
        Transform pivot = arr[high];
        int i = low - 1;

        for (int j = low; j < high; j++)
        {
            if (Vector3.Distance(transform.position, arr[j].position)
                < Vector3.Distance(transform.position, pivot.position))
            {
                i++;
                Swap(arr, i, j);
            }
        }
        Swap(arr, i + 1, high);
        return i + 1;
    }

    private void Swap(List<Transform> arr, int a, int b)
    {
        (arr[a], arr[b]) = (arr[b], arr[a]);
    }
    //patrol 끝--------------------------

    //Chase 관련
    private void Chase()
    {

        if (agentRemainingDistance > 7f)
        {
            BackToHome();
            
            switch (StayMob)
            {
                case true:
                    MonsterState = State.IDLE;
                    break;
                default:
                    MonsterState = State.Patrol;
                    break;
            }
        }
        else
        {
            MoveTo(Near);
        }
    }
    private void BackToHome()
    {
        MoveTo(homePoint);
    }



    //Attack 관련---------------
  
    private void Attack()
    {
        attacktimer += Time.deltaTime; // timer > 1f = 1초 이상

        if (attacktimer > attackDelay)//공격 딜레이 설정
        {
            //공격
            attacktimer = 0;
            MoveTo(Near);
        }
    }

    private void Stop()
    {
        MoveTo(transform.position);
    }
    public void ChangeState(State _state)
    {
        MonsterState = _state;
    }
    //Attack 끝------------

    

}
