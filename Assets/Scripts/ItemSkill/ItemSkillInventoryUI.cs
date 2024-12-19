using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkillInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;

    [SerializeField] private GameObject selectSlot;
    private int _prevIndex;

   
    void Start()
    {   
        //Initalize
        SelectToggle(0);
    }

    public void SelectToggle(int index)
    {
        selectSlot.transform.SetParent(slots[index].transform);
        selectSlot.transform.localPosition = Vector3.zero;
        
        _prevIndex = index;
    }
    
    public void SetItemSprite(int index, int itemIndex)
    {
        if (itemIndex == -1)
        {
            slots[index].transform.GetChild(2).GetComponent<Image>().enabled = false;
            return;
        }

        var slotImage = slots[index].transform.GetChild(2).GetComponent<Image>();
        slotImage.sprite = ItemManager.Instance.GetItemSprite(itemIndex);
                                                                
        slotImage.enabled = true;
    }
}
