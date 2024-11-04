using Fusion;
using Fusion.Addons.KCC;
using System;
using UnityEngine;

/// <summary>
/// Health Ŭ������ ĳ������ ü���� �����ϴ� ������ �����մϴ�.
/// </summary>
public class Health : NetworkBehaviour
{
    public float MaxHealth = 100f; // �ִ� ü��

    public bool IsAlive => CurrentHealth > 0f; // ĳ���Ͱ� ����ִ��� ���θ� ��ȯ�ϴ� ������Ƽ

    [Networked]
    public float CurrentHealth { get; private set; } // ���� ü��, ��Ʈ��ũ ����ȭ��

    /// <summary>
    /// ��Ʈ��ũ ��ü�� ������ �� ȣ��Ǵ� �޼����, ������ ���� ���¿��� ü���� �ִ�ġ�� �����մϴ�.
    /// </summary>
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            CurrentHealth = MaxHealth; // ������ �ִ� ��� �ʱ� ü���� �ִ� ü������ ����
        }
    }

    /// <summary>
    /// �������� �����ϴ� �޼���
    /// </summary>
    /// <param name="playerRef">�������� ���� �÷��̾� ����</param>
    /// <param name="damage">������ ������ ��</param>
    /// <returns>�������� ���������� ����Ǿ����� ����</returns>
    public bool ApplyDamage(PlayerRef playerRef, float damage)
    {
        // ü���� �̹� 0 ���϶�� ������ ���� �Ұ�
        if (CurrentHealth <= 0f)
        {
            return false;
        }

        CurrentHealth -= damage; // ��������ŭ ü�� ����
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f; // ü���� 0 ���ϰ� �Ǹ� 0���� ����
            Debug.Log("Dead"); // ��� �޽��� ���
        }
        return true; // ������ ���� ����
    }

    /// <summary>
    /// ü���� ȸ���ϴ� �޼���
    /// </summary>
    /// <param name="health">ȸ���� ü�� ��</param>
    /// <returns>ȸ���� ���������� �̷�������� ����</returns>
    public bool AddHealth(float health)
    {
        // ü���� �̹� 0 �����̰ų� �ִ� ü���� �ʰ��ϸ� ȸ�� �Ұ�
        if (CurrentHealth <= 0f)
            return false;
        if (CurrentHealth >= MaxHealth)
            return false;

        // ü���� ȸ���ϵ�, �ִ� ü���� �ʰ����� �ʵ��� ����
        CurrentHealth = Mathf.Min(CurrentHealth + health, MaxHealth);

        return true; // ȸ�� ����
    }
}
