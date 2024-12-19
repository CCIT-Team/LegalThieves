using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGroup : MonoBehaviour
{
    [SerializeField] ItemBase[] PlayerItems;

    void Start(){
        PlayerItems = GetComponentsInChildren<ItemBase>();
    }

     public ItemBase GetItemClass(int itemIndex)
    {
        return PlayerItems[itemIndex];
    }
   
}
