
using Fusion;
using UnityEngine;


public enum EItemType
{
    Empty = -1,
    Torch,
    Flashlight,
    Count
}

public class ItemManager : NetworkBehaviour
{
    public static ItemManager Instance;

    [SerializeField] public ItemBase[] items;
    [SerializeField] private Sprite[] itemSprites;
    
    public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            //SpawnAllRelics();
        }
        
    private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }
    public ItemBase GetItemClass(int itemIndex)
    {
        return items[itemIndex];
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

