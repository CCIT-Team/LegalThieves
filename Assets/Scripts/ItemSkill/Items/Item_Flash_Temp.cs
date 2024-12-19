
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;
using UnityEngine.XR;

public class Item_Flash_Temp : ItemBase
{
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject flashObject;
     void Start()
    {
        ID = (int)EItemType.Flashlight;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        Debug.Log("useitem");
        TurnOnOffLight();
        Debug.Log("0");
    }
    public override void EquipItem(Animator animator)
    {
        flashObject.SetActive(true); 
        
    }
    public override void UnequipItem(Animator animator)
     { 
        flashObject.SetActive(false); 
    }
    #endregion

    public void TurnOnOffLight()
    {
        IsActivity = !IsActivity;
        if (IsActivity)
        {
              Debug.Log("1");
            flashLight.SetActive(true);
        }
        else
        { Debug.Log("12");
            flashLight.SetActive(false);
        }
    }

}
