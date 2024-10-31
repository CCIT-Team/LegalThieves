using Fusion;
using Fusion.Addons.KCC;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public float MaxHealth = 100f;

    public bool IsAlive => CurrentHealth > 0f;

    [Networked]
    public float CurrentHealth { get; private set; }

    private void Start()
    {
        
        CurrentHealth = MaxHealth;
    }



    public void HealZone(float heal)
    {
        if(CurrentHealth < MaxHealth)
        {
            CurrentHealth += heal;
        }
    }
    public void PoisonZone(float damage)
    {
        if(CurrentHealth > 0)
        {
            CurrentHealth -= damage;
        }
    }
}
