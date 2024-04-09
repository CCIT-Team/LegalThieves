using UnityEngine;

    public class MobChase : State
    {
        
        private Vector3 homePoint;
        private bool isBack;
        public override void Enter()
        {
            if (homePoint == Vector3.zero)
            {
                homePoint = transform.position;
            }

        }
        
        public override void LogicUpdate()
        {
            base.LogicUpdate();
        
            Chase();
        }
        //플레이어 감지
        private void OnTriggerEnter(Collider other)
        {
            Monster.monsterMoveSM.ChangeState(Monster.MobChase);
        
        }
        //위치 돌아감
        private void BackToHome()
        {
            Monster.agent.SetDestination(homePoint);
        }
        
        //플레이어를 향해 이동
        private void Chase() 
        { 
           
            if (Monster.agent.remainingDistance > 7f )
            {
                BackToHome();
                homePoint = Vector3.zero;
                switch(Monster.StayMob)
                {
                    case true: 
                        Monster.monsterMoveSM.Initialize(Monster.MobIdle);
                        break;
                    default:
                        Monster.monsterMoveSM.Initialize(Monster.MobPatrol);
                        break;
                }
            }
            else
            {
                Monster.agent.SetDestination(Monster.player.position);
            }

        }
    }

