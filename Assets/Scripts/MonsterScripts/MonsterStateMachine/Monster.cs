using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class Monster : MonsterBase
{
 
    public enum State { IDLE, Chase, Patrol, Attack };

    State state;
    public State getState
    {
        get { return state; }
    }

    bool isFirst = true;
    //Patrol property
    private float timer;
    private int PatrolDelay = 2;
    public Transform[] patrolPoints;
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

  

    // Update is called once per frame
    private void Start()
    {
        patrolPoints = GameObject.FindWithTag("MobPatrollParent").GetComponentsInChildren<Transform>();
        switch (StayMob)
        {
            case true:
                state = State.IDLE;
                break;
            case false:
                state = State.Patrol;
                break;
        }

        Init();
    }

    private void Update()
    {
        switch(state)
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
                Stop();
                Attack();
                break;

        }
     
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (state)
        {
            case State.IDLE:
                Near = CheckNearPlayer();
                state = State.Chase;
               
                break;
            case State.Patrol:
                Near = CheckNearPlayer();
                state = State.Chase;

                break;
           
            case State.Attack:
                break;

        }
    
    }

    void IDLE()
    {
        //대기 애니메이션
    }


    //patrol 관련---------------------------------

    private void Patrol()
    {
        if (agent.velocity != Vector3.zero) return;
        Debug.Log("실행");
        timer += Time.deltaTime;

        if (timer > PatrolDelay)
        {
            timer = 0;
            NearPointSet(patrolPoints, 0, patrolPoints.Length - 1);
            agent.SetDestination(patrolPoints[Random.Range(2, 3)].position);
        }
    }

    private void NearPointSet(Transform[] arr, int low, int high)
    {
        if (low < high)
        {
            int pivotIndex = Partition(arr, low, high);
            NearPointSet(arr, low, pivotIndex - 1);
            NearPointSet(arr, pivotIndex + 1, high);
        }
    }

    private int Partition(Transform[] arr, int low, int high)
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

    private void Swap(Transform[] arr, int a, int b)
    {
        (arr[a], arr[b]) = (arr[b], arr[a]);
    }
    //patrol 끝--------------------------

    //Chase 관련
    private void Chase()
    {

        if (agent.remainingDistance > 7f)
        {
            BackToHome();
            homePoint = Vector3.zero;
            switch (StayMob)
            {
                case true:
                    state = State.IDLE;
                    break;
                default:
                    state = State.Patrol;
                    break;
            }
        }
        else
        {
            agent.SetDestination(Near.position);
        }
    }
    private void BackToHome()
    {
        agent.SetDestination(homePoint);
    }



    //Attack 관련---------------
  
    private void Attack()
    {
        attacktimer += Time.deltaTime; // timer > 1f = 1초 이상

        if (attacktimer > attackDelay)//공격 딜레이 설정
        {
            //공격
            attacktimer = 0;
           agent.SetDestination(Near.position);
        }
    }

    private void Stop()
    {
        agent.SetDestination(transform.position);
    }
    public void ChangeState(State _state)
    {
        state = _state;
    }
    //Attack 끝------------

    

}
