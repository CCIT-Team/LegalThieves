
using UnityEngine;

public class Item_Flash_Temp : ItemBase
{
    
    public override void UseItem() {TurnOnLight(); }
    public override void EquipItem() { }
    public override void UnequipItem() { }

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
