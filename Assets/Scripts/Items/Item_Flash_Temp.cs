using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Flash_Temp : MonoBehaviour
{
   
    [SerializeField] Light flashLight;
    public void TurnOnLight()
    {
        flashLight.enabled = true;
   
    }

    public void TurnOffLight()
    {
        flashLight.enabled = false;
    }

}
