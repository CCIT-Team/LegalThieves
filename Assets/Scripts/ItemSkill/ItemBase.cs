
using UnityEngine;
using System.Collections;
public abstract class ItemBase : MonoBehaviour
{
    public int ID;
    public int itemPrice;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    public bool IsActivity; // 도구 사용중 여부 ex. 손전등이 켜져있는가?
    public AudioSource itemSFX;

    public Coroutine animationCoroutine;

    public abstract void UseItem(Animator animator);
    public abstract void EquipItem(Animator animator);
    public abstract void UnequipItem(Animator animator);

    protected IEnumerator ChangeObjectAfterDelay(GameObject itemObject, bool isVisible,float delay)
    {
        yield return new WaitForSeconds(delay);
        
        itemObject.SetActive(isVisible);
        animationCoroutine = null;
    }
}
