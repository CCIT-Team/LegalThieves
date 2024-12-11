using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] string itemName;
    [SerializeField] Sprite itemIcon;
    [SerializeField] bool IsVisible;
    [SerializeField] bool IsActive;
    [SerializeField] AudioSource itemSFX;

    public virtual void UseItem() { }
    public virtual void EquipItem() { }
    public virtual void UnequipItem() { }
}
