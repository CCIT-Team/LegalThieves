
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public int ID;
    public string itemName;
    public Sprite itemIcon;
    public bool IsVisiblity; //렌더링 여부
    public bool IsActivity; // 도구 사용중 여부 ex. 손전등이 켜져있는가?
    public AudioSource itemSFX;

    public abstract void UseItem(Animator animator);
    public abstract void EquipItem(Animator animator);
    public abstract void UnequipItem(Animator animator);
}
