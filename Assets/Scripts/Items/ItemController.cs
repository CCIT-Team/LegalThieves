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

public interface IItem
{
    public void UseItem();
    public void EquipItem();
    public void UnequipItem();
}


public class ItemController : MonoBehaviour
{
    [SerializeField] private IItem[] items;
    [SerializeField] private EItemType currentItem = EItemType.Null;
 
    
    public void UseItem()
    {
        items[(int)currentItem].UseItem();
    }

    public void EquipItem(EItemType itemType)
    {
        if(currentItem != EItemType.Null)
        items[(int)currentItem].UnequipItem();

        currentItem = itemType;
        items[(int)currentItem].EquipItem();

        
    }

    public void UnequipItem()
    {
        items[(int)currentItem].UnequipItem();
    }
}
