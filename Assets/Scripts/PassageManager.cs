using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PassageManager : MonoBehaviour
{
    public GameObject passageTrap;

    private void OnEnable()
    {
        passageTrap = Instantiate(passageTrap);
        passageTrap.transform.parent = this.transform;
    }
}
