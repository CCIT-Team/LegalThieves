using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    ItemBase currentItem;

    public void UseItem(){
        if(currentItem == null) return;
        Debug.Log(currentItem.name);
        currentItem.UseItem();
    }
     public void EquipItem(int itemIndex) {
        if (itemIndex==-1) 
        {
         currentItem.UnequipItem(); 
         currentItem = null;
        }
        else
        {
        currentItem = ItemManager.Instance.GetItemClass(itemIndex);
        currentItem.EquipItem();
        }

    }
    public void UnequipItem() { 
        currentItem.UnequipItem(); 
        }
    
    public void ConsumingItems(){
           currentItem = null;
    }

}
