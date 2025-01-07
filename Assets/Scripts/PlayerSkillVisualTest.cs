using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillVisualTest : MonoBehaviour
{
    [Header("풀스크린쉐이더")]
    [SerializeField] private Material fullscreenMaterial;

    [Header("풀스크린쉐이더 프로퍼티")]
    [Range (0f, 5f)][SerializeField] private float contrastIntensity = 1f;
    [Range(0f, 1f)][SerializeField] private float grayscaleFactor = 0f;
    [Range(0f, 1f)][SerializeField] private float InvertColorFactor = 0f;
    [SerializeField] private float value = 1f;
    [Range(0, 1)][SerializeField] private int isVignette = 0;
    [SerializeField] private Color vignetteColor;
    [SerializeField] private float vignettePower = 5f;
    [SerializeField] private float vignetteIntensity = 10f;

    private void Update()
    {
        //Test function for editor
        SetInspectorValues();


    }

    private void SetInspectorValues()
    {
        //Test function for editor
        fullscreenMaterial.SetFloat("_ContrastIntensity", contrastIntensity);
        fullscreenMaterial.SetFloat("_GrayscaleFactor", grayscaleFactor);
        fullscreenMaterial.SetFloat("_InvertColorFactor", InvertColorFactor);
        fullscreenMaterial.SetFloat("_Value", value);
        fullscreenMaterial.SetInt("_Vignette", isVignette);
        fullscreenMaterial.SetColor("_VignetteColor", vignetteColor);
        fullscreenMaterial.SetFloat("_VignettePower", vignettePower);
        fullscreenMaterial.SetFloat("_VignetteIntensity", vignetteIntensity);
    }
}
