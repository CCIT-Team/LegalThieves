
using UnityEngine;
using System.Collections;
using Fusion;

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

        if (itemIndex == -1)
        {
            if (currentItem == null) return;
            currentItem.UnequipItem(animator);
            currentItem = null;
        }
        else
        {
            if(currentItem == null){
                currentItem = ItemGroup.GetItemClass(itemIndex);
                currentItem.EquipItem(animator);
            }
            else {
            currentItem.UnequipItem(animator);
            StartCoroutine(ChangeItem(animator, itemIndex));}

        }
    }
    IEnumerator ChangeItem(Animator animator, int itemIndex)
    {
        if (currentItem.animationCoroutine == null)
        {
                currentItem = ItemGroup.GetItemClass(itemIndex);
                currentItem.EquipItem(animator);
                StopCoroutine(ChangeItem(animator, itemIndex));
        }else
            {
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(ChangeItem(animator, itemIndex));
            }
    }

    public void ConsumingItems()
    {
        currentItem = null;
    }
    public void SetHolder(Transform itemHolder)
    {

        ItemGroup.transform.parent = itemHolder;
        ItemGroup.transform.transform.localPosition = Vector3.zero;
    }

}
