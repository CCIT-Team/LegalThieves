using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Torch_Temp : ItemBase
{
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float changingTime = 3f;

    [SerializeField] private ParticleSystem[] torchParticleSystems;
    [SerializeField] private Light torchLight;

    [SerializeField] private GameObject torchObject;


    Coroutine torchCoroutine ;

    void Start()
    {
        ID = (int)EItemType.Torch;
    }
    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        Debug.Log("useitem");
        TurnOnOffLight();
    }

    public override void EquipItem(Animator animator) { torchObject.SetActive(true);}
    public override void UnequipItem(Animator animator) {torchObject.SetActive(false); }
    #endregion
    public void TurnOnOffLight()
    {
        if (torchCoroutine !=null) return;

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
            StartCoroutine(ChangeLightIntensity(-1f));

            foreach (ParticleSystem p in torchParticleSystems)
            {
                var emission = p.emission;
                emission.enabled = false;
            }
            //  AudioManager.instance.PlayTorchLoopSfx(false);
        }
   
    }

    private IEnumerator ChangeLightIntensity(float delta)
    {
        torchLight.intensity += delta * lightIntensity / changingTime * Time.deltaTime;

        yield return null;

        if (delta == 1f && torchLight.intensity < lightIntensity)
        {
            StartCoroutine(ChangeLightIntensity(delta));
        }
        else if (delta == -1f && torchLight.intensity > 0f)
        {
            StartCoroutine(ChangeLightIntensity(delta));
        }
        else {
            StopCoroutine(ChangeLightIntensity(delta));
            torchCoroutine = null;
        }
     
    }
}
