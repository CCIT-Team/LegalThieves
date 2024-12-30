
using UnityEngine;
using System.Collections;
using Fusion;
using Unity.VisualScripting;

public class PlayerItemController : NetworkBehaviour
{
    ItemBase currentItem;
    private ItemGroup ItemGroup;
    public void SetItemGroup()
    {
        ItemGroup = Instantiate(ItemManager.Instance.ItemGroupOrigin, Vector3.zero, Quaternion.identity).GetComponent<ItemGroup>();
    }
    public void UseItem(Animator animator)
    {
        if (currentItem == null || currentItem.animationCoroutine != null) return;

        Debug.Log(currentItem.name);
        currentItem.UseItem(animator);
    }
    public void EquipItem(Animator animator, int itemIndex)
    {
        if (currentItem != null && currentItem.animationCoroutine != null) return; 

        if (itemIndex == -1)//빈공간을 선택할때
        {
            if (currentItem == null) return;  //지금 아이템을 안들고있다면 리턴턴
            currentItem.UnequipItem(animator);
            currentItem = null;
        }
        else     // 아이템을 선택할떄
        {
            if(currentItem == null){ // 지금 아이템을 안들고 있다면
                currentItem = ItemGroup.GetItemClass(itemIndex);
                currentItem.EquipItem(animator);
            }
            else {  // 들고 있다면
            currentItem.UnequipItem(animator);
            ChangeItem(animator, itemIndex);
            }
        }
    }
    void ChangeItem(Animator animator, int itemIndex)
    {
            currentItem = ItemGroup.GetItemClass(itemIndex);
            currentItem.EquipItem(animator);

    }

    public void ConsumingItem()
    {
        currentItem = null;
    }
    public void SetHolder(Transform itemHolder)
    {

        ItemGroup.transform.parent = itemHolder;
        ItemGroup.transform.localPosition = Vector3.zero;
        ItemGroup.transform.localRotation = Quaternion.identity;
    }

}
