
using UnityEngine;
using System.Collections;
using New_Neo_LT.Scripts.UI;
public abstract class ItemBase : MonoBehaviour
{
    public int ID;
    public int itemPrice;
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;

    protected bool IsActivity; // 도구 사용중 여부 ex. 손전등이 켜져있는가?
    protected bool canUse = true;
    public bool CanUse { get { return canUse; } }
    public AudioSource itemSFX;
    public Coroutine animationCoroutine;
    public float baseDelay;

    public abstract void UseItem(Animator animator);
    public abstract void EquipItem(Animator animator);
    public abstract void UnequipItem(Animator animator);

     public abstract void Init();
     
    protected IEnumerator ChangeObjectAfterDelay(GameObject itemObject, bool isVisible, float delay)
    {
        yield return new WaitForSeconds(delay);

        itemObject.SetActive(isVisible);
        animationCoroutine = null;
    }

    protected IEnumerator CoolDownDelay()
    {
        yield return new WaitForSeconds(baseDelay);
        canUse = true;
    }
 
  


}
