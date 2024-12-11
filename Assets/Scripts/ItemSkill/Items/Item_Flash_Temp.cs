
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

public class Item_Flash_Temp : ItemBase
{
    bool isOn;
    [SerializeField] GameObject flashLight;

    #region ItemBaseLogic
    public override void UseItem()
    {
       TurnOnOffLight();
    }
    public override void EquipItem() {
        
        //FlashScript[CurrentPlayerModelIndex].gameObject.SetActive(IsFlashVisibility);
    }
    public override void UnequipItem() { }
    #endregion

    public void TurnOnOffLight()
    {
        if (isOn)
            flashLight.SetActive(true);
        else
            flashLight.SetActive(false);
    }
}
