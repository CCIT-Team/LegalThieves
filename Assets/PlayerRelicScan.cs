
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.Relic;
using New_Neo_LT.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
public class PlayerRelicScan : MonoBehaviour
{
    PlayerCharacter character;


    void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Relics"))
        {
            if (!Application.isFocused) return;
            
            //  UIManager.Instance.scanUI.OnScanUI();
            UIManager.Instance.scanUI.SetUIPoint(other.GetComponent<RelicObject>().RelicID);
        
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Relics"))
        {
            if (!Application.isFocused) return;

            UIManager.Instance.scanUI.SetUIPoint(-1);
          //  UIManager.Instance.scanUI.OffScanUI();
        }
    }
  
}
