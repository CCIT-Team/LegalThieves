using System.Collections.Generic;
using System.Linq;
using New_Neo_LT.Scripts.UI;
using UnityEngine;

public enum EItemType
{
    Empty = -1,
    Torch,
    Flashlight,
    WoodStick,
    Compass,
    GPSDevice,
    Count
}

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public GameObject ItemGroupOrigin;
    public ItemBase[] items;
    
    [SerializeField] private Sprite emptySprite;

    private readonly Dictionary<int, ItemBase> _itemDictionary = new();
    
    public ItemBase[] ItemBases => _itemDictionary.Values.ToArray();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        var ig = ItemGroupOrigin.GetComponentsInChildren<ItemBase>();

        foreach (var ib in ig)
        {
            ib.Init();
            _itemDictionary.Add(ib.ID, ib);
        }
        
        UIManager.Instance.marketUIController.InitUI();
    }
    
    public int GetItemID(int index)
    {
        return items[index].ID;
    }

    public Sprite GetItemSprite(int iD)
    {
        var ib = _itemDictionary[iD];
        
        return ib != null ? ib.itemIcon : emptySprite;
    }
    
    public int GetItemPrice(int iD)
    {
        var ib = _itemDictionary[iD];
        
        return ib != null ? ib.itemPrice : 0;
    }

    public string GetItemName(int iD)
    {
        var ib = _itemDictionary[iD];
        
        return ib != null ? ib.itemName : "???";
    }
    
    public string GetItemDescription(int iD)
    {
        var ib = _itemDictionary[iD];
        
        return ib != null ? ib.itemDescription : "???";
    }
}

