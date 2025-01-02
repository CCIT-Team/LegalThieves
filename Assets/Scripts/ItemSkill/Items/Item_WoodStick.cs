using System;
using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_WoodStick : ItemBase
{

    [SerializeField] private float hitDelay = 1f;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private TrailRenderer swingTrail;
    [SerializeField] private GameObject stickObject;
    [SerializeField] private BoxCollider hitColl;

    bool canSwing=true;
    public override void Init()
    {
        ID = (int)EItemType.WoodStick;
        hitColl.enabled = false;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        SwingCheck(animator);
    }

    public override void EquipItem(Animator animator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(stickObject, true, 1f));
        animator.SetBool("pickStick", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickStick", false);
         hitColl.enabled =  false;
        IsActivity = false;
       animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(stickObject, false, 1f));
    }
    #endregion

    private void SwingCheck(Animator animator)
    {
        if(canSwing)
        animationCoroutine = StartCoroutine(SwingAction(animator));
    }

    private IEnumerator SwingAction(Animator animator){
        canSwing = false;
        hitColl.enabled = true;
        swingTrail.enabled = true;
        animator.SetTrigger("UseItem");
        yield return new WaitForSeconds(1f);
        canSwing = true;
        hitColl.enabled = false;
        swingTrail.enabled = false;
        animationCoroutine = null;
    }
}

