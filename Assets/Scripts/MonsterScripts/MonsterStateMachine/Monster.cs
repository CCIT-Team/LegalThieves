using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Monster : MonsterBase
{

    private StateMachine _monsterMoveSM;
    public StateMachine monsterMoveSM { get { return _monsterMoveSM;} }
    
    private MobIDLE _MobIdle;
    private MobChase _MobChase;
    private MobAttack _MobAttack;
    private MobPatrol _MobPatrol;
    
    public MobIDLE MobIdle { get { return _MobIdle;} }
    public MobChase MobChase { get { return _MobChase;} }
    public MobAttack MobAttack { get { return _MobAttack;} }
    public MobPatrol MobPatrol { get { return _MobPatrol;} }

    [SerializeField]
    private bool _StayMob;
    public bool StayMob { get { return _StayMob;} }
    // Update is called once per frame
    private void Start()
    {
        _monsterMoveSM = GetComponent<StateMachine>();
        _MobIdle = GetComponent<MobIDLE>(); // 순찰 없는 몬스터만 사용
        _MobChase = GetComponent<MobChase>();
        _MobAttack = GetComponent<MobAttack>();
        _MobPatrol = GetComponent<MobPatrol>();

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
