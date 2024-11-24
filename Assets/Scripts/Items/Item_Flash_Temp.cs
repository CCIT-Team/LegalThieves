using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Flash_Temp : MonoBehaviour
{
   
    [SerializeField] GameObject flashLight;
    public void TurnOnLight()
    {
        flashLight.SetActive(true);
   
    }

    public void TurnOffLight()
    {
        flashLight.SetActive(false);
    }

}
