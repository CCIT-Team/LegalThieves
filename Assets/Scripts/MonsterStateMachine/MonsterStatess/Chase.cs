using System;
using UnityEngine;
namespace MonsterStateMachine.States
{
    public class Chase : State
    {
        public Chase():base() {}

        public override void Exit()
        {  
            base.Exit();
            Monster.BackToSpawn();
        }
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
            Monster.MobBaseMove();
        }
        
        
    
    }
}
