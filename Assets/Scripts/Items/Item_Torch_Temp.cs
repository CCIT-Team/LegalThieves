using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Torch_Temp : MonoBehaviour
{
    [SerializeField] private float lightIntensity = 1f;
    [SerializeField] private float changingTime = 3f;

    [SerializeField] private ParticleSystem[] torchParticleSystems;
    [SerializeField] private Light torchLight;

    void Start()
    {
        torchLight = this.GetComponentInChildren<Light>();
        torchParticleSystems = this.GetComponentsInChildren<ParticleSystem>();
    }

    public void TurnOnLight()
    {
        StartCoroutine(ChangeLightIntensity(1f));

        foreach (ParticleSystem p in torchParticleSystems)
        {
            var emission = p.emission;
            emission.enabled = true;
        }
        AudioManager.instance.PlayTorchLoopSfx(true);
    }

    public void TurnOffLight()
    {
        StartCoroutine(ChangeLightIntensity(-1f));

        foreach (ParticleSystem p in torchParticleSystems)
        {
            var emission = p.emission;
            emission.enabled = false;
        }
        AudioManager.instance.PlayTorchLoopSfx(false);
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
