
using UnityEngine;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEditor;

public class PlayerItemController : MonoBehaviour
{
    [SerializeField] GameObject ItemGroup;
    ItemBase currentItem;

    [SerializeField] Transform[] itemHolders = new Transform[4];
    Transform currentItemHolder;

    public void UseItem(Animator animator)
    {
        if (currentItem == null) return;

        currentItem.IsActivity = !currentItem.IsActivity;
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
            currentItem = ItemManager.Instance.GetItemClass(itemIndex);
          
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
    public void SetHolder(int index){
        currentItemHolder = itemHolders[index];
        ItemGroup.transform.parent = currentItemHolder;
        ItemGroup.transform.transform.position=Vector3.zero;
     }
    
}
