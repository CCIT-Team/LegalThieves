using Fusion;
using Fusion.Addons.KCC;
using System;
using UnityEngine;

/// <summary>
/// Health 클래스는 캐릭터의 체력을 관리하는 역할을 수행합니다.
/// </summary>
public class Health : NetworkBehaviour
{
    public float MaxHealth = 100f; // 최대 체력

    public bool IsAlive => CurrentHealth > 0f; // 캐릭터가 살아있는지 여부를 반환하는 프로퍼티

    [Networked]
    public float CurrentHealth { get; private set; } // 현재 체력, 네트워크 동기화됨

    /// <summary>
    /// 네트워크 객체가 생성될 때 호출되는 메서드로, 권한을 가진 상태에서 체력을 최대치로 설정합니다.
    /// </summary>
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CurrentHealth = MaxHealth; // 권한이 있는 경우 초기 체력을 최대 체력으로 설정
        }
    }

    /// <summary>
    /// 데미지를 적용하는 메서드
    /// </summary>
    /// <param name="playerRef">데미지를 가한 플레이어 참조</param>
    /// <param name="damage">적용할 데미지 값</param>
    /// <returns>데미지가 성공적으로 적용되었는지 여부</returns>
    public bool ApplyDamage(PlayerRef playerRef, float damage)
    {
        // 체력이 이미 0 이하라면 데미지 적용 불가
        if (CurrentHealth <= 0f)
        {
            return false;
        }

        CurrentHealth -= damage; // 데미지만큼 체력 감소
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f; // 체력이 0 이하가 되면 0으로 설정
            Debug.Log("Dead"); // 사망 메시지 출력
        }
        return true; // 데미지 적용 성공
    }

    /// <summary>
    /// 체력을 회복하는 메서드
    /// </summary>
    /// <param name="health">회복할 체력 값</param>
    /// <returns>회복이 성공적으로 이루어졌는지 여부</returns>
    public bool AddHealth(float health)
    {
        // 체력이 이미 0 이하이거나 최대 체력을 초과하면 회복 불가
        if (CurrentHealth <= 0f)
            return false;
        if (CurrentHealth >= MaxHealth)
            return false;

        // 체력을 회복하되, 최대 체력을 초과하지 않도록 설정
        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);

        return true; // 회복 성공
    }
}
