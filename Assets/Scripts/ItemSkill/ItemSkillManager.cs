using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkillManager : MonoBehaviour
{
    [SerializeField] public ItemBase[] Items;
    [SerializeField] public SkillBase[] Skills;

    [SerializeField] private EItemType currentItem = EItemType.Null;

    public void UseItem()
    {
        if(currentItem == EItemType.Null) return;

        Items[(int)currentItem].UseItem();
    }

    public void EquipItem(EItemType itemType)
    {
        if (currentItem != EItemType.Null)
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

