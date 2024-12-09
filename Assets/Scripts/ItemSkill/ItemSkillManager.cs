using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Null = -1,
    Torch,
    Flashlight,
    Count
}
public enum ESKillType
{
    Null = -1,
    Torch,
    Flashlight,
    Count
}

public interface IItem
{
    public void UseItem();
    public void EquipItem();
    public void UnequipItem();
}

public interface ISkill
{
    public void UseSkill();
}

public class ItemSkillManager : MonoBehaviour
{
    [SerializeField] public IItem[] Items;
    [SerializeField] public ISkill[] Skills;

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

