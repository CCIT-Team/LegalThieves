using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSkillInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject[] slots;
    [SerializeField] private int[] slotIndex = new int[6];
    [SerializeField] private GameObject selectSlot;
    private int _prevIndex;

   
    void Start()
    {   
        //Initalize
        for (int i = 0; i < slotIndex.Length; i++)
        {
            slotIndex[i] = -1;
        }
        slotIndex[1] = 0;
        SelectToggle(0);
        
    }

    public void SelectToggle(int index)
    {
        selectSlot.transform.SetParent(slots[index].transform);
        selectSlot.transform.localPosition = Vector3.zero;
        
        _prevIndex = index;
    }
    
    public void SetRelicSprite(int index, int itemIndex)
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
