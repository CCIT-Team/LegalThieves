using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Resource,
    Equipable,
    Consumable,
    Relic 
}

// 소모 아이템 사용 시 변경될 Conditions
public enum ConsumableType
{
    Hunger,
    Health
}

[Serializable]
public class ItemDataConsumable
{
    public ConsumableType type;
    public float value;
}

[CreateAssetMenu(fileName = "Item", menuName = "New Item")]
public class itemData_k : ScriptableObject
{
    [Header("Info")]
    public string displayName;
    public string description;
    public ItemType type;
    public Sprite icon;
    public GameObject dropPerfab;

    [Header("Stacking")]
    public bool canStack;
    public int maxStackAmount;

    // 소모 가능한 아이템의 체력과 배고픔 데이터
    [Header("Consumable")]
    public ItemDataConsumable[] consumables;
}