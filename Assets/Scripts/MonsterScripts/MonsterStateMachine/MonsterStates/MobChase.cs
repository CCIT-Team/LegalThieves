using UnityEngine;

    public class MobChase : State
    {
        public MobChase():base() {}
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
        public void BackToHome()
        {
            Monster.agent.SetDestination(homePoint);
        }
        
        //플레이어를 향해 이동
        public void Chase() 
        { 
            if (Vector3.Distance(transform.position, Monster.player.position) > 7f )
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

