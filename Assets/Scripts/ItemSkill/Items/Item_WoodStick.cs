using System;
using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_WoodStick : ItemBase
{

    [SerializeField] private float hitDelay = 1f;
    
    [SerializeField] private TrailRenderer swingTrail;
    [SerializeField] private GameObject stickObject;
    [SerializeField] private BoxCollider hitColl;

    [SerializeField] private TrailRenderer swingTrailLocal;
    [SerializeField] private GameObject stickObjectLocal;


    bool canSwing = true;
    public override void Init()
    {
        ID = (int)EItemType.WoodStick;
        hitColl.enabled = false;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator, Animator armAnimator)
    {
        SwingCheck(animator, armAnimator);
    }

    public override void EquipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickStick", true);
        armAnimator?.SetBool("pickStick", true);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(stickObject, true, 1f));

            StartCoroutine(ChangeObjectAfterDelay(stickObjectLocal, true, 1f));
    }
    public override void UnequipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickStick", false);
        armAnimator?.SetBool("pickStick", false);

        hitColl.enabled = false;
        IsActivity = false;

        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(stickObject, false, 1f));
 
            StartCoroutine(ChangeObjectAfterDelay(stickObjectLocal, false, 1f));
    }
    #endregion

    private void SwingCheck(Animator animator, Animator armAnimator)
    {
        if (canSwing)
            animationCoroutine = StartCoroutine(SwingAction(animator, armAnimator));
    }

    private IEnumerator SwingAction(Animator animator, Animator armAnimator)
    {
        canSwing = false;
        hitColl.enabled = true;
        swingTrail.enabled = true;
        swingTrailLocal.enabled = swingTrail.enabled;
        animator.SetTrigger("UseItem");
        armAnimator?.SetTrigger("UseItem");
        yield return new WaitForSeconds(1f);
        canSwing = true;
        hitColl.enabled = false;
        swingTrail.enabled = false;
        swingTrailLocal.enabled = swingTrail.enabled;
        animationCoroutine = null;
    }
}

