using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class PlayerConditions_k : MonoBehaviour
{
    public Condition health;
    public Condition stamina;


    public UnityEvent onTakeDamage;     // 데미지를 받았을 때 처리할 이벤트를 받기 위한 Unity Event

    // Start is called before the first frame update
    void Start()
    {
        health.curValue = health.startValue;
        stamina.curValue = stamina.startValue;
    }

    // Update is called once per frame
    void Update()
    {
        // 1초마다 스테미나 차게 만들기
        stamina.Add(stamina.regenRate * Time.deltaTime);     

        // 체력이 다 닳으면 캐릭터 죽음
        if (health.curValue == 0.0f)
        {
            Die();
        }

        // Bar 변경하기
        health.uiBar.fillAmount = health.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    

    public bool UseStamina(float amount)
    {
        // 스테미나가 없을 경우 스테미나를 사용하지 못하도록
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
        // Condition의 값이 추가가 될 때 추가된 현재 값이 최대값이 넘지 않도록, 최대값보다 클 경우 최댓값을 현재 Value로 변경해준다.
        curValue = Mathf.Min(curValue + amount, maxValue);
    }

    public void Subtract(float amount)
    {
        // Condtion의 값이 차감이 될 때 현재 값이 최소값을 넘지 않도록, 최소값보다 작을 경우 최소값을 현재 Value로 변경해준다.
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    // UI에서 Fill Amount를 사용할 때, 0 ~ 1사이의 Percentage를 사용한다. 
    public float GetPercentage()
    {
        // 현재 값을 최대 값으로 나눠서 0 ~ 1 사이의 Percentage를 return 해준다.
        return curValue / maxValue;
    }
}