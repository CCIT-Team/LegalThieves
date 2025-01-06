
using UnityEngine;
using System.Collections;
public class Item_Flash : ItemBase
{
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject flashObject;

    [SerializeField] GameObject flashObjectLocal;
    public override void Init()
    {
        ID = (int)EItemType.Flashlight;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator, Animator armAnimator)
    {
        TurnOnOffLight();
    }
    public override void EquipItem(Animator animator, Animator armAnimator)
    {

        animator.SetBool("pickFlash", true);
        armAnimator?.SetBool("pickFlash", true);

        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(flashObject, true, 1f));

        StartCoroutine(ChangeObjectAfterDelay(flashObjectLocal, true, 1f));
 
    }
    public override void UnequipItem(Animator animator, Animator armAnimator)
    {
        IsActivity = false;
        flashLight.SetActive(false);
        animator.SetBool("pickFlash", false);
        armAnimator?.SetBool("pickFlash", false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(flashObject, false, 1f));

        StartCoroutine(ChangeObjectAfterDelay(flashObjectLocal, false, 1f));
      
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


}