
using UnityEngine;
using New_Neo_LT.Scripts.PlayerComponent;

public class PlayerItemController : MonoBehaviour
{
    public Animator animator;
    ItemBase currentItem;
    public void SetItemAnimator(Animator player) {
            animator = player;
    }
    public void UseItem(){
        if(currentItem == null) return;
        Debug.Log(currentItem.name);
        currentItem.UseItem(animator);
    }
     public void EquipItem(int itemIndex) {
        if (itemIndex==-1) 
        {
         currentItem.UnequipItem(animator); 
         currentItem = null;
        }
        else
        {
        currentItem = ItemManager.Instance.GetItemClass(itemIndex);
        currentItem.EquipItem(animator);
        }

    }
    public void UnequipItem() { 
        currentItem.UnequipItem(animator); 
        }
    
    public void ConsumingItems(){
           currentItem = null;
    }

}
