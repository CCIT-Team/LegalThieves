using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionProgressUITest : MonoBehaviour
{
    [SerializeField] private GameObject testingUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            testingUI.SetActive(true);
        }
    }
}
