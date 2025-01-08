
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
public class Item_GPSDevice : ItemBase
{
    [SerializeField] GameObject GPSDeviceObject;
    [SerializeField] Animator GPSAnimator;
    [SerializeField] Animator GPSDeviceObjectLocalAnim;
    [SerializeField] float effectTime = 5;
    [SerializeField] UniversalRendererData Outline;

    public override void Init()
    {
        ID = (int)EItemType.GPSDevice;
        Outline.rendererFeatures[1].SetActive(false);
        Outline.rendererFeatures[2].SetActive(false);

    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator, Animator armAnimator)
    {
        UsePlayerScan(animator, armAnimator);
    }
    public override void EquipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickGPS", true);
        armAnimator?.SetBool("pickGPS", true);
             GPSAnimator.SetTrigger("On");
        GPSDeviceObjectLocalAnim.SetTrigger("On");
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObject, true, 1f));

        StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObjectLocalAnim.gameObject, true, 1f));

    }
    public override void UnequipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickGPS", false);
        armAnimator?.SetBool("pickGPS", false);
        GPSAnimator.SetTrigger("Off");
        GPSDeviceObjectLocalAnim.SetTrigger("Off");
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObject, false, 1f));


        StartCoroutine(ChangeObjectAfterDelay(GPSDeviceObjectLocalAnim.gameObject, false, 1f));
    }
    #endregion

    public void UsePlayerScan(Animator animator, Animator armAnimator)
    {

        animator.SetTrigger("UseItem");
        armAnimator?.SetTrigger("UseItem");
        animationCoroutine = StartCoroutine(PlayerScan());
        canUse = false;
        StartCoroutine(CoolDownDelay());

    }

    private IEnumerator PlayerScan()
    {
   
        yield return new WaitForSeconds(1f);
        GPSAnimator.SetTrigger("Scan");
        GPSDeviceObjectLocalAnim.SetTrigger("Scan");
        Outline.rendererFeatures[1].SetActive(true);
        Outline.rendererFeatures[2].SetActive(true);
        yield return new WaitForSeconds(effectTime);
       
        Outline.rendererFeatures[1].SetActive(false);
        Outline.rendererFeatures[2].SetActive(false);
        animationCoroutine = null;
    }
}