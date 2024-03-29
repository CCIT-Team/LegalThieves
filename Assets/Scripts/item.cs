using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject //오브젝트에 안넣어도 효력이 있음
{
    public string itemName;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;

    public enum ItemType { Equip, Used, Ingredient, ETC }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
