using System.Collections;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Torch_Temp : ItemBase
{
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float changingTime = 3f;

    [SerializeField] private ParticleSystem[] torchParticleSystems;
    [SerializeField] private Light torchLight;

    void Start()
    {
        ID = (int)EItemType.Torch;    
    }
 #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        TurnOnLight();
    }

    public override void EquipItem(Animator animator) { TurnOffLight(); }
    public override void UnequipItem(Animator animator) { }
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
    }

    public void TurnOffLight()
    {
        StartCoroutine(ChangeLightIntensity(-1f));

        foreach (ParticleSystem p in torchParticleSystems)
        {
            var emission = p.emission;
            emission.enabled = false;
        }
        //  AudioManager.instance.PlayTorchLoopSfx(false);
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
        else StopCoroutine(ChangeLightIntensity(delta));
    }
}
