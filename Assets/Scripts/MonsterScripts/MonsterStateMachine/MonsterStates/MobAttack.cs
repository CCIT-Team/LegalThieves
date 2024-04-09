using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MobAttack : State
{
  
   private float timer=0;
   private int attackDelay=1;

   public override void Enter()
   {
       base.Enter();
       AttackStop();
   }

   public override void LogicUpdate()
   {
       base.LogicUpdate();
       Attack();
   }

   public override void Exit()
   {
       timer = 0;
   }

   private void Attack()
   {
       timer += Time.deltaTime; // timer > 1f = 1초 이상
   
       if(timer > attackDelay)//공격 딜레이 설정
       {
           //공격
           timer = 0;
       }
   }

   private void AttackStop()
   {
       Monster.agent.SetDestination(transform.position);
   }
}
