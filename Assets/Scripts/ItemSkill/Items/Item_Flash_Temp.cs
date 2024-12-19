
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;
using UnityEngine.XR;

public class Item_Flash_Temp : ItemBase
{
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject flashRenderObject;
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
        flashRenderObject.SetActive(true);
        
    }
    public override void UnequipItem(Animator animator)
     { 
         flashRenderObject.SetActive(false);
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
