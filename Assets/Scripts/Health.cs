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

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CurrentHealth = MaxHealth;
        }
    }

    public bool ApplyDamage(PlayerRef playerRef, float damage)
    {
        if(CurrentHealth <= 0f)
        {
            return false;
        }
        CurrentHealth -= damage;
        if(CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            Debug.Log("Dead");
        }
        return true;
    }
    public bool AddHealth(float health)
    {
        if(CurrentHealth <= 0f)
            return false;
        if(CurrentHealth >= MaxHealth)
            return false;

        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);   

        return true;
    }
}
