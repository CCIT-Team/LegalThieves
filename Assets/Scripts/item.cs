using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject //오브젝트에 안넣어도 효력이 있음
{
<<<<<<< HEAD
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
=======
    public enum Type { Artifact, ArtifactLocation };
    public Type type;
    public float price;  // 유물의 가격
    public int year;   // 유물이 발견된 년도
    public float weight;  // 유물의 무게
>>>>>>> item
}
