using System;
using UnityEngine;



public class MobAttackArea : MonoBehaviour
{
    private Monster Monster;
    int playerCount;
    private void Awake()
    {
        Monster = GetComponentInParent<Monster>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount++;
            Monster.ChangeState(Monster.State.Attack);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerCount--;
            if(playerCount == 0)
            Monster.ChangeState(Monster.State.Chase);
        }
    }
}

