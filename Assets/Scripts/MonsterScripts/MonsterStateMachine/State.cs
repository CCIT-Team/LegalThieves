using UnityEngine;


    public class State : MonoBehaviour
    {

    protected Transform Near;

    protected Transform CheckNearPlayer()
    {
        GameObject[] players = PlayerManager.Instance.PlayerArray;
        GameObject near = players[0];

        for (int i = 1; i < players.Length; i++)
        {
            if (Vector3.Distance(transform.position, near.transform.position) >
                Vector3.Distance(transform.position, players[i].transform.position))
            {
                near = players[i];
            }
        }
        return near.transform;
    }

    public Monster Monster;
    
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

