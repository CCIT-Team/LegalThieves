using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class NewBehaviourScript : MonoBehaviour
{
    public Slider HpBar;
    
   
    void Update()
    {
        Hp();
    }

    void Hp()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            HpBar.value -= 0.1f;
        }
    }
}
