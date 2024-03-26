using System;
using UnityEngine;

namespace MonsterStateMachine
{
    public class MobAttackArea : MonoBehaviour
    {
        private Monster Monster;

        private void Awake()
        {
            Monster = GetComponentInParent<Monster>();
        }

        void OnTriggerEnter(Collider orther)
        {
            Monster.monsterMoveSM.ChangeState(Monster.MobAttack);
        }

        private void OnTriggerExit(Collider other)
        {
            Monster.monsterMoveSM.ChangeState(Monster.MobChase);
            
        }
    }
}
