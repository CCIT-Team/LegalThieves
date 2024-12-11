
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] string itemName;
    [SerializeField] Sprite itemIcon;
    [SerializeField] bool IsVisible;
    [SerializeField] bool IsActive;
    [SerializeField] AudioSource itemSFX;

    public abstract void UseItem();
    public abstract void EquipItem();
    public abstract void UnequipItem(); 
}
