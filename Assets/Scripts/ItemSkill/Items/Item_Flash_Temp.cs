
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Flash_Temp : ItemBase
{
    [SerializeField] GameObject flashLight;
    [SerializeField] GameObject flashRenderObject;
    void Start()
    {
        ID = (int)EItemType.Flashlight;
        animator = PlayerCharacter.Local.GetAnimator();
    }

    #region ItemBaseLogic
    public override void UseItem()
    {
        TurnOnOffLight();
    }
    public override void EquipItem()
    {
        flashRenderObject.SetActive(true);
        
    }
    public override void UnequipItem() { 
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
