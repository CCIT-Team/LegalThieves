using System.Collections;
using System.Collections.Generic;
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

    void Awake(){
        if (Instance == null)
        {
            Instance = this;
        }
        else { Destroy(gameObject); }
    }
    [SerializeField] public ItemBase[] Items;
    [SerializeField] private EItemType currentItem = EItemType.Empty;

    public ItemBase GetItemClass(int itemIndex)
    {
        return Items[itemIndex];
    }

   
}

