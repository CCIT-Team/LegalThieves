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
    Coroutine hitCoroutine;
    bool canSwing;
    void Start()
    {
        ID = (int)EItemType.WoodStick;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        SwingCheck(animator);
    }

    public override void EquipItem(Animator animator)
    {
        stickObject.SetActive(true);
        animator.SetBool("pickTorch", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickTorch", false);

        IsActivity = false;
        hitCoroutine = null;
        animationCoroutine = StartCoroutine(UnequipStick(animator));
    }
    #endregion

    private IEnumerator UnequipStick(Animator animator)
    {
        while (true)
        {
            var animatorState = animator.GetCurrentAnimatorStateInfo(2);
            if (animatorState.IsName("TorchIdle"))
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                stickObject.SetActive(false);
                break;
            }
        }
        animationCoroutine = null;
    }

    private void SwingCheck(Animator animator)
    {
        var animatorState = animator.GetCurrentAnimatorStateInfo(2);
        if (!animatorState.IsName("TorchAttack"))
        {
           StartCoroutine(SwingAction(animator));
        }
    }

    private IEnumerator SwingAction(Animator animator)
    {
        var anim = animator.GetCurrentAnimatorStateInfo(2);

        if (anim.IsName("TorchAttack")) yield return null;

        animator.SetBool("Attack",true);
        while (anim.IsName("TorchAttack")){
             yield return new WaitForSeconds(0.5f);
        }

    }
}

