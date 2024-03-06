using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightInput : MonoBehaviour
{
    [SerializeField] private GameObject[] lights;
    [SerializeField][Range(0f, 30f)] private float lightChangingSpeed;
    [SerializeField][Range(0f, 5f)] private float lightChangingIntensity;

    private bool isThereTorch;
    private float xValue;

    private void Start()
    {
        foreach (GameObject light in lights)
        {
            light.SetActive(false);
        }
        lightChangingSpeed = 6f;
        lightChangingIntensity = 0.3f;
        isThereTorch = false;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject light in lights)
            {
                light.SetActive(false);
            }
            isThereTorch = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            lights[0].SetActive(true);
            lights[1].SetActive(false);
            isThereTorch = true;
            StartCoroutine(ChangingTorchLight());
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            lights[0].SetActive(false);
            lights[1].SetActive(true);
            isThereTorch = false;
        }
    }

    private IEnumerator ChangingTorchLight()
    {
        Light touchLight = lights[0].GetComponent<Light>();
        touchLight.range = 5f - (Mathf.Sin(xValue * Mathf.PI * lightChangingSpeed) * lightChangingIntensity);

        xValue += Time.deltaTime;

        yield return null;

        if (!isThereTorch)
        {
            xValue = 0f;
            StopCoroutine(ChangingTorchLight());
        }

        StartCoroutine(ChangingTorchLight());
    }
}
