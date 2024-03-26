using System.Collections;
using System.Collections.Generic;
using MonsterStateMachine;
using UnityEngine;

public class Attack : State
{
   public Attack():base() {}

   public override void Enter()
   {
       base.Enter();
       Monster.Stop();
   }

   public override void LogicUpdate()
   {
       base.LogicUpdate();
       Monster.Attack();
   }

   public override void Exit()
   {
       Monster.timer = 0;
   }
}
