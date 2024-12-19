
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;
using UnityEngine.XR;

public class Item_Flash_Temp : ItemBase
{
    [SerializeField] GameObject flashLight;
   
    void Start()
    {
        ID = (int)EItemType.Flashlight;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator)
    {
        Debug.Log("useitem");
        TurnOnOffLight();
    }
    public override void EquipItem(Animator animator)
    {
        gameObject.SetActive(false); 
        
    }
    public override void UnequipItem(Animator animator)
     { 
        gameObject.SetActive(true); 
    }
    #endregion

    public void TurnOnOffLight()
    {
        IsActivity = !IsActivity;
        if (IsActivity)
        {
            flashLight.SetActive(true);
        }
        else
        {
            flashLight.SetActive(false);
        }
    }

}
