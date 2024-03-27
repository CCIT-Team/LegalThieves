using System.Collections;
using System.Collections.Generic;
using MonsterStateMachine;
using UnityEngine;

public class Patrol : State
{
    public override void Enter()
    {
        base.Enter();
        //두리번두리번 애니메이션 
        //순찰위치로 이동
    }

    public override void Exit()
    {
        base.Exit();
        Monster.monsterMoveSM.ChangeState(Monster.MobIdle);
    }
}
