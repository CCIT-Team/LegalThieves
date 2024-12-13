
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public int ID;
    public string itemName;
    public Sprite itemIcon;
    public bool IsVisiblity; //렌더링 여부
    public bool IsActivity; // 도구 사용중 여부 ex. 손전등이 켜져있는가?
    public AudioSource itemSFX;
    public Animator animator;

    public abstract void UseItem();
    public abstract void EquipItem();
    public abstract void UnequipItem();
}
