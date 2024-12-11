using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerItemController : MonoBehaviour
{
    ItemBase currentItem;
    Animator animator;
    public void UseItem(){
        currentItem.UseItem();
    }
     public void EquipItem(int itemIndex) {
        currentItem = ItemManager.Instance.GetItemClass(itemIndex);
        currentItem.EquipItem();
    }
    public void UnequipItem() { 
        currentItem.UnequipItem(); }
}
