
using UnityEngine;

using Fusion;
using New_Neo_LT.Scripts.UI;


public class PlayerItemController : NetworkBehaviour
{
    ItemBase currentItem;
    private ItemGroup ItemGroup;

    [SerializeField] Animator[] armAnimators;
    [SerializeField] Animator currentArmAnimator;
    public void SetItemGroup()
    {
        ItemGroup = Instantiate(ItemManager.Instance.ItemGroupOrigin, Vector3.zero, Quaternion.identity).GetComponent<ItemGroup>();
    }
    public void SetLocalItemGroup()
    {

        ItemGroup.SetLocalItemGroup();

    }
    public void HideItem()
    {
        if(HasInputAuthority)
        {ItemGroup.HideItem();}
    }
    public void UseItem(int slotIndex, Animator animator)
    {
        if (currentItem == null || currentItem.animationCoroutine != null || !currentItem.CanUse) return;
        currentItem.UseItem(animator, currentArmAnimator);
       
        if (HasInputAuthority)
        {
            UIManager.Instance.itemSkillInventoryUI.CoolDownSlotUI(slotIndex, currentItem.baseDelay);
        }
    }
    public void EquipItem(Animator animator, int itemIndex)
    {
        if (currentItem != null && currentItem.animationCoroutine != null)
        {
            currentItem.UnequipItem(animator, currentArmAnimator);
            ChangeItem(animator, itemIndex);
        }

        if (itemIndex == -1)//빈공간을 선택할때
        {
            if (currentItem == null) return;  //지금 아이템을 안들고있다면 리턴턴
            currentItem.UnequipItem(animator, currentArmAnimator);
            currentItem = null;
        }
        else     // 아이템을 선택할떄
        {
            if (currentItem == null)
            { // 지금 아이템을 안들고 있다면
                currentItem = ItemGroup.GetItemClass(itemIndex);
                currentItem.EquipItem(animator, currentArmAnimator);
            }
            else
            {  // 들고 있다면
                currentItem.UnequipItem(animator, currentArmAnimator);
                ChangeItem(animator, itemIndex);
            }
        }
    }
    void ChangeItem(Animator animator, int itemIndex)
    {
        currentItem = ItemGroup.GetItemClass(itemIndex);
        currentItem.EquipItem(animator, currentArmAnimator);

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


    public void SetArmAnimator(int index)
    {
        currentArmAnimator = armAnimators[index];

        if (HasInputAuthority)
        {

            foreach (var anim in armAnimators)
            {
                anim.gameObject.SetActive(false);
            }
            currentArmAnimator.gameObject.SetActive(true);
        }
    }

}
