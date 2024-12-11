using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESKillType
{
    Null = -1,
    Torch,
    Flashlight,
    Count
}

public enum EItemType
{
    Empty = -1,
    Torch,
    Flashlight,
    Count
}

public class ItemSkillManager : MonoBehaviour
{
    [SerializeField] public ItemBase[] Items;
    [SerializeField] public SkillBase[] Skills;

    [SerializeField] private EItemType currentItem = EItemType.Empty;

    public void UseItem()
    {
        if(currentItem == EItemType.Empty) return;

        Items[(int)currentItem].UseItem();
    }

    public void EquipItem(EItemType itemType)
    {
        if (currentItem != EItemType.Empty)
            Items[(int)currentItem].UnequipItem();

        currentItem = itemType;
        Items[(int)currentItem].EquipItem();
    }

    public void UnEquipItem()
    {
        Items[(int)currentItem].UnequipItem();
    }

    public void UseSkill(ESKillType skillType)
    {
        
        Skills[(int)skillType].UseSkill();
    }
}

