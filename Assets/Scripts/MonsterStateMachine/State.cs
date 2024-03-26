using UnityEngine;

namespace MonsterStateMachine
{
    public class State : MonoBehaviour
    {
        protected Monster Monster;
    
        // Update is called once per frame
        void Awake(){
            Monster = transform.GetComponent<Monster>();
        }

        public virtual void Enter()
        {
       
        }

        public virtual void HandleInput()
        {

        }

        public virtual void LogicUpdate()
        {

        }

        public virtual void PhysicsUpdate()
        {

        }

        public virtual void Exit()
        {

        }
    }
}
