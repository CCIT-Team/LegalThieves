
using UnityEngine;
using System.Collections;
public class Item_GPSDevice : ItemBase
{
//todo 네비메쉬 이용해서 길찾기 보여주는거 구현, 아이템 목록에 추가, 애니메이션 추가
    [SerializeField] GameObject GPSDeviceObject;
     [SerializeField] float delay=30;
   
    void Start()
    {
        ID = (int)EItemType.GPSDevice;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        UsePlayerScan(animator);
    }
    public override void EquipItem(Animator animator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObject, true, 1f));
        animator.SetBool("pickGPS", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickGPS", false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObject, false, 1f));
    }
    #endregion

    public void UsePlayerScan(Animator animator)
    {
        if(canUse)
        { 
            animator.SetTrigger("UseItem");
            canUse = false;
            StartCoroutine(Delay(delay));
        }
    }   
  
   
}