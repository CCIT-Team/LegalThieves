
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
        animationCoroutine = StartCoroutine(FlashTurnOff(animator));
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

    private IEnumerator FlashTurnOff(Animator animator)
    {
        var animatorState = animator.GetCurrentAnimatorStateInfo(2);
        if (animatorState.IsName("FlashUnequip"))
        {
            yield return new WaitForSeconds(1.0f);
        }

        flashObject.SetActive(false);
        animationCoroutine = null;
    }
}
