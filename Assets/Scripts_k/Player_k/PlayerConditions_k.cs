using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class PlayerConditions_k : MonoBehaviour
{
    public Condition health;
    public Condition stamina;


    public UnityEvent onTakeDamage;     // �������� �޾��� �� ó���� �̺�Ʈ�� �ޱ� ���� Unity Event

    // Start is called before the first frame update
    void Start()
    {
        health.curValue = health.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        // 1�ʸ��� ���׹̳� ���� �����
        stamina.Add(stamina.regenRate * Time.deltaTime);     

        // ü���� �� ������ ĳ���� ����
        if (health.curValue == 0.0f)
        {
            Die();
        }

        // Bar �����ϱ�
        health.uiBar.fillAmount = health.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    

    public bool UseStamina(float amount)
    {
        // ���׹̳��� ���� ��� ���׹̳��� ������� ���ϵ���
        if (stamina.curValue - amount < 0) return false;

        stamina.Subtract(amount);
        return true;
    }

    public void Die()
    {
        Debug.Log("Player Die!");
    }
}
[Serializable]
public class Condition
{
    [HideInInspector]
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)
    {
        // Condition�� ���� �߰��� �� �� �߰��� ���� ���� �ִ밪�� ���� �ʵ���, �ִ밪���� Ŭ ��� �ִ��� ���� Value�� �������ش�.
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        // Condtion�� ���� ������ �� �� ���� ���� �ּҰ��� ���� �ʵ���, �ּҰ����� ���� ��� �ּҰ��� ���� Value�� �������ش�.
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    // UI���� Fill Amount�� ����� ��, 0 ~ 1������ Percentage�� ����Ѵ�. 
    public float GetPercentage()
    {
        // ���� ���� �ִ� ������ ������ 0 ~ 1 ������ Percentage�� return ���ش�.
        return curValue / maxValue;
    }
}