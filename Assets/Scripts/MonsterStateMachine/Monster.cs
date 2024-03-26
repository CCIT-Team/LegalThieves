using System.Collections;
using System.Collections.Generic;
using MonsterStateMachine.States;
using MonsterStateMachine;
using UnityEngine;

public class Monster : MonsterBase
{
    public float timer;
    int attackDelay;
    
    public StateMachine monsterMoveSM;
    
    public IDLE MobIdle;
    public Chase MobChase;
    public Attack MobAttack;

    private Vector3 spawnPoint;
    
    // Update is called once per frame
    private void Start()
    {
        monsterMoveSM = GetComponent<StateMachine>();
        MobIdle = GetComponent<IDLE>();
        MobChase = GetComponent<Chase>();
        MobAttack = GetComponent<Attack>();
        
        monsterMoveSM.Initialize(MobIdle);
        
        spawnPoint = transform.position;

        attackDelay = 1;
        timer = 0;
        Init();
    }

    private void Update()
    {
        monsterMoveSM.currentState.LogicUpdate();
    }

    //트리거
    private void OnTriggerEnter(Collider other)
    {
        monsterMoveSM.ChangeState(MobChase);
        
    }
    // 행동정의------------
    
    //위치 돌아감
    public void BackToSpawn()
    {
        agent.SetDestination(spawnPoint);
    }
    //플레이어를 향해 이동
    public void MobBaseMove() 
    { 
        // 플레이어에게 다가감
        agent.SetDestination(player.position);
        if (Vector3.Distance(transform.position, player.position) > 7f)
        {
            monsterMoveSM.ChangeState(MobIdle);
        }
    }
    
    public void Stop()
    {
        agent.SetDestination(transform.position);
    }
    
    public void Attack()
    {
        timer += Time.deltaTime;
   
        if(timer > attackDelay)
        {
            Debug.Log("공격"+timer);
            timer = 0;
        }
    }
}
