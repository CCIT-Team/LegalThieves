using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Monster : MonsterBase
{

    public StateMachine monsterMoveSM;
    
    public MobIDLE MobIdle;
    public MobChase MobChase;
    public MobAttack MobAttack;
    public MobPatrol MobPatrol;

    public bool StayMob;
    // Update is called once per frame
    private void Start()
    {
        monsterMoveSM = GetComponent<StateMachine>();
        MobIdle = GetComponent<MobIDLE>(); // 순찰 없는 몬스터만 사용
        MobChase = GetComponent<MobChase>();
        MobAttack = GetComponent<MobAttack>();
        MobPatrol = GetComponent<MobPatrol>();

        switch(StayMob)
        {
            case true: 
                monsterMoveSM.Initialize(MobIdle);
                break;
            default:
                monsterMoveSM.Initialize(MobPatrol);
                break;
        }

        Init();
    }

    private void Update()
    {
        monsterMoveSM.currentState.LogicUpdate();
    }

}
