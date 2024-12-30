using System;
using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_WoodStick : ItemBase
{

    [SerializeField] private float hitDelay = 1f;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private TrailRenderer SwingTrail;
    [SerializeField] private GameObject stickObject;
    [SerializeField] private BoxCollider hitColl;
    Coroutine hitCoroutine;
    bool canSwing=true;
    void Start()
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
        animationCoroutine = StartCoroutine(ChangeFlashObjectAfterDelay(stickObject, true, 1f));
        animator.SetBool("pickStick", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickStick", false);

        IsActivity = false;
        hitCoroutine = null;
       animationCoroutine = StartCoroutine(ChangeFlashObjectAfterDelay(stickObject, false, 1f));
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
        SwingTrail.enabled = true;
        animator.SetTrigger("Attack");
        yield return new WaitForSeconds(1f);
        canSwing = true;
        hitColl.enabled = false;
        SwingTrail.enabled = false;
        animationCoroutine = null;
    }
}

