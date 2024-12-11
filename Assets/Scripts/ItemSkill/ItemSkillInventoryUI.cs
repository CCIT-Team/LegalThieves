using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        SelectToggle(0);
    }

    public void SelectToggle(int index)
    {
        selectSlot.transform.SetParent(slots[index].transform);
        selectSlot.transform.localPosition = Vector3.zero;
        

        _prevIndex = index;
    }
    
    public void SetRelicSprite(int index, int relicIndex)
    {
        // if (relicIndex == -1)
        // {
        //     slots[index].transform.GetChild(2).GetComponent<Image>().enabled = false;
        //     return;
        // }

        // var slotImage = slots[index].transform.GetChild(2).GetComponent<Image>();
        // slotImage.sprite = ItemSkillManager.Instance.GetRelicSprite(RelicManager.Instance.GetRelicData(relicIndex).GetRelicType(),
        //                                                         RelicManager.Instance.GetRelicData(relicIndex).GetTypeIndex());
        // slotImage.enabled = true;
    }


}
