using System;
using UnityEngine;



    public class MobAttackArea : MonoBehaviour
    {
        private Monster Monster;

        private void Awake()
        {
            Monster = GetComponentInParent<Monster>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Monster.monsterMoveSM.ChangeState(Monster.MobAttack);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            Monster.monsterMoveSM.ChangeState(Monster.MobChase);
            
        }
    }

