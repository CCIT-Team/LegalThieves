
using UnityEngine;
using System.Collections;
public class Item_Compass : ItemBase
{
//todo 네비메쉬 이용해서 길찾기 보여주는거 구현, 아이템 목록에 추가, 애니메이션 추가
    [SerializeField] GameObject CompassObject;
     [SerializeField] float delay=10;
      public override void Init()
    {
        ID = (int)EItemType.Compass;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        UsePathFinding(animator);
    }
    public override void EquipItem(Animator animator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, true, 1f));
        animator.SetBool("pickCompass", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickCompass", false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, false, 1f));
    }
    #endregion

    public void UsePathFinding(Animator animator)
    {
        if(canUse)
        { 
            animator.SetTrigger("UseItem");
            canUse = false;
            StartCoroutine(CoolDownDelay(delay));
        }
    }   

   
}