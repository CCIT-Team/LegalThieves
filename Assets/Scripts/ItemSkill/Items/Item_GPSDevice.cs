
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using System;
using New_Neo_LT.Scripts.UI;
public class Item_GPSDevice : ItemBase
{
//todo 네비메쉬 이용해서 길찾기 보여주는거 구현, 아이템 목록에 추가, 애니메이션 추가
    [SerializeField] GameObject GPSDeviceObject;
     [SerializeField] float effectTime=5;
    [SerializeField] UniversalRendererData Outline;
    public override void Init()
    {
        ID = (int)EItemType.GPSDevice;
        Outline.rendererFeatures[1].SetActive(false);   
        Outline.rendererFeatures[2].SetActive(false);   
       
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
    
            animator.SetTrigger("UseItem");
            animationCoroutine = StartCoroutine(PlayerScan());
            canUse = false;
            StartCoroutine(CoolDownDelay());
        
    }

    private IEnumerator PlayerScan()
    {
        yield return new WaitForSeconds(1f);
      
        Outline.rendererFeatures[1].SetActive(true);   
        Outline.rendererFeatures[2].SetActive(true);   
        yield return new WaitForSeconds(effectTime);

        Outline.rendererFeatures[1].SetActive(false);   
        Outline.rendererFeatures[2].SetActive(false);  
        animationCoroutine=null; 
    }
}