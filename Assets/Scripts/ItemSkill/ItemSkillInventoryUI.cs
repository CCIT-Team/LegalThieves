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
            slots[index].transform.GetChild(1).GetComponent<Image>().enabled = false;
            return;
        }

        var slotImage = slots[index].transform.GetChild(1).GetComponent<Image>();
        slotImage.sprite = ItemManager.Instance.GetItemSprite(itemIndex);

        slotImage.enabled = true;
    }

    public void CoolDownSlotUI(int index, float delay){
        StartCoroutine(CoolDownSlotRoutine(index, delay));
    }
    IEnumerator CoolDownSlotRoutine(int index, float delay)
    {
        if (delay == 0) yield return null;
        var step = 1/delay;
        var slot =  slots[index].GetComponent<Slider>();
        Debug.Log(slot.value);
        for (float i = 0; i < 1; i += step)
        {
            Debug.Log(slot.value);
            slot.value = 1-i;
            yield return new WaitForSeconds(1f);
        }
    }
}
