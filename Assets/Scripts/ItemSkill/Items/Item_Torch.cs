using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Torch : ItemBase
{
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float changingTime = 3f;
    [SerializeField] private ParticleSystem[] torchParticleSystems;
    [SerializeField] private Light torchLight;
    [SerializeField] private GameObject torchObject;

    Coroutine torchCoroutine;

    void Start()
    {
        ID = (int)EItemType.Torch;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        TurnOnOffLight();
    }

    public override void EquipItem(Animator animator)
    {
        animationCoroutine = StartCoroutine(ChangeFlashObjectAfterDelay(torchObject, true, 1f));
        animator.SetBool("pickTorch", true);
    }
    public override void UnequipItem(Animator animator)
    {
        animator.SetBool("pickTorch", false);
        OffLight();
        IsActivity = false;
        torchCoroutine = null;
        animationCoroutine = StartCoroutine(ChangeFlashObjectAfterDelay(torchObject, false, 1f));
    }

 
    #endregion

    public void TurnOnOffLight()
    {
        if (torchCoroutine != null) return;

        IsActivity = !IsActivity;
        if (IsActivity)
        {
            torchCoroutine = StartCoroutine(ChangeLightIntensity(1f));

            foreach (ParticleSystem p in torchParticleSystems)
            {
                var emission = p.emission;
                emission.enabled = true;
            }
            //   AudioManager.instance.PlayTorchLoopSfx(true);
        }
        else
        {
            torchCoroutine = StartCoroutine(ChangeLightIntensity(-1f));

            foreach (ParticleSystem p in torchParticleSystems)
            {
                var emission = p.emission;
                emission.enabled = false;
            }
            //  AudioManager.instance.PlayTorchLoopSfx(false);
        }

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
            torchCoroutine = null;
        }
    }
    void OffLight(){
        torchLight.intensity = 0;
         foreach (ParticleSystem p in torchParticleSystems)
            {
                var emission = p.emission;
                emission.enabled = false;
            }
    }

}


