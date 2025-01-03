using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Torch : ItemBase
{
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float changingTime = 3f;
    [SerializeField] private float swingDelay = 5;
    [SerializeField] private ParticleSystem[] torchParticleSystems;
    [SerializeField] private Light torchLight;
    [SerializeField] private GameObject torchObject;
    [SerializeField] private BoxCollider hitColl;
    [SerializeField] private TrailRenderer swingTrail;


    public override void Init()
    {
        canUse = true;
        ID = (int)EItemType.Torch;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
            Swing(animator);
    }

    public override void EquipItem(Animator animator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(torchObject, true, 1f));
        animator.SetBool("pickTorch", true);
        TurnOnLight();
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickTorch", false);
        TurnOffLight();
        hitColl.enabled = false;
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(torchObject, false, 1f));
    }


    #endregion

    public void TurnOnLight()
    {

        StartCoroutine(ChangeLightIntensity(1f));

        foreach (ParticleSystem p in torchParticleSystems)
        {
            var emission = p.emission;
            emission.enabled = true;
        }
        //   AudioManager.instance.PlayTorchLoopSfx(true);

        IsActivity = true;

    }
    void TurnOffLight()
    {
        torchLight.intensity = 0;
        foreach (ParticleSystem p in torchParticleSystems)
        {
            var emission = p.emission;
            emission.enabled = false;
        }
        IsActivity = false;

    }

    private IEnumerator ChangeLightIntensity(float delta, Animator animator = null)
    {
        torchLight.intensity += delta * lightIntensity / changingTime * Time.deltaTime;

        yield return null;

        if (delta == 1f && torchLight.intensity < lightIntensity)
        {
            StartCoroutine(ChangeLightIntensity(delta, animator));
        }
        else if (delta == -1f && torchLight.intensity > 0f)
        {
            StartCoroutine(ChangeLightIntensity(delta, animator));
        }
        else
        {
            StopCoroutine(ChangeLightIntensity(delta, animator));
        }
    }

    private void Swing(Animator animator)
    {
        animationCoroutine = StartCoroutine(SwingAction(animator));
        StartCoroutine(CoolDownDelay());
    }

    private IEnumerator SwingAction(Animator animator)
    {
        canUse = false;
        hitColl.enabled = true;
        swingTrail.enabled = true;
        animator.SetTrigger("UseItem");
        yield return new WaitForSeconds(1f);
        hitColl.enabled = false;
        swingTrail.enabled = false;
        animationCoroutine = null;

    }
}


