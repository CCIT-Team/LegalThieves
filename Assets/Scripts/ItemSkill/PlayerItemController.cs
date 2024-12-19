
using UnityEngine;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEditor;
using Fusion;
using Unity.Mathematics;

public class PlayerItemController : NetworkBehaviour
{
    ItemBase currentItem;
    private ItemGroup ItemGroup;

    public void SetItemGroup()
    {
        ItemGroup = Instantiate(ItemManager.Instance.ItemGroupOrigin,Vector3.zero,Quaternion.identity).GetComponent<ItemGroup>();
    }
    public void UseItem(Animator animator)
    {
        if (currentItem == null) return;

        Debug.Log(currentItem.name);
        currentItem.UseItem(animator);
    }
    public void EquipItem(Animator animator, int itemIndex)
    {
        if ( itemIndex == -1)
        {
            if (currentItem == null) return;
            currentItem.UnequipItem(animator);
            currentItem = null;
        }
        else
        {
            currentItem = ItemGroup.GetItemClass(itemIndex);
            currentItem.IsVisiblity = true;
                  Debug.Log("equip");
            currentItem.EquipItem(animator);
        }
    }
    public void UnequipItem(Animator animator)
    {
        currentItem.UnequipItem(animator);
        currentItem.IsVisiblity = false;
        
    }
    public void ConsumingItems()
    {
        currentItem = null;
    }
    public void SetHolder(Transform itemHolder){
 
        ItemGroup.transform.parent = itemHolder;
        ItemGroup.transform.transform.localPosition=Vector3.zero;
    }
    
}
