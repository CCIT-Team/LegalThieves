
using UnityEngine;
using System.Collections;
public class Item_Flash_Temp : ItemBase
{
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject flashObject;
    void Start()
    {
        ID = (int)EItemType.Flashlight;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {

        TurnOnOffLight();

    }
    public override void EquipItem(Animator animator)
    {
        flashObject.SetActive(true);
        animator.SetBool("pickFlash", true);

    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickFlash", false);
        flashLight.SetActive(false);
        IsActivity = false;
        animationCoroutine = StartCoroutine(FlashObjectOff(animator));
    }
    #endregion

    public void TurnOnOffLight()
    {
        IsActivity = !IsActivity;
        if (IsActivity)
        {
            flashLight.SetActive(true);
        }
        else
        {
            flashLight.SetActive(false);
        }
    }

    private IEnumerator FlashObjectOff(Animator animator)
    {
        while (true)
        {
            var animatorState = animator.GetCurrentAnimatorStateInfo(2);
            if (animatorState.IsName("FlashUnequip"))
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                flashObject.SetActive(false);
                break;
            }
        }
        animationCoroutine = null;

    }
}