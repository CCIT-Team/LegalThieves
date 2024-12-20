
using Fusion;
using UnityEngine;

public enum EItemType
{
    Empty = -1,
    Torch,
    Flashlight,
    Count
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public GameObject ItemGroupOrigin;
    public ItemBase[] items;

    private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);

            items = ItemGroupOrigin.GetComponentsInChildren<ItemBase>();
        }

    public int GetItemID(int index)
    {
        return items[index].ID;
    }

    public Sprite GetItemSprite(int index)
    {
        return items[index].itemIcon;
    }
    public string GetItemName(int index)
    {
        return items[index].itemName;
    }
   
}

