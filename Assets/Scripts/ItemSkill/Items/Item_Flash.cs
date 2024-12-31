
using UnityEngine;
using System.Collections;
public class Item_Flash : ItemBase
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
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(flashObject, true, 1f));
        animator.SetBool("pickFlash", true);
    }
    public override void UnequipItem(Animator animator)
    {
        IsActivity = false;
        animator.SetBool("pickFlash", false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(flashObject, false, 1f));
   
 
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