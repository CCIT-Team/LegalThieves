using System;
using LegalThieves;
using UnityEngine;
using UnityEngine.UI;

namespace New_Neo_LT.Scripts.UI
{
    public class InventorySlotController : MonoBehaviour
    {
        [SerializeField] private GameObject[] slots;
        private int _prevIndex;

        private void Start()
        {
            SelectToggle(0);
        }

        public void SelectToggle(int index)
        {
            slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(false);
            slots[index].transform.GetChild(1).gameObject.SetActive(true);
            _prevIndex = index;
        }

        public void SetRelicSprite(int index, int relicIndex)
        {
            if(relicIndex == -1)
            {
                slots[index].transform.GetChild(2).GetComponent<Image>().enabled = false;
                return;
            }
            
            var slotImage = slots[index].transform.GetChild(2).GetComponent<Image>();
            slotImage.sprite = RelicManager.Instance.GetRelicSprite(RelicManager.Instance.GetRelicData(relicIndex).GetTypeIndex());
            slotImage.enabled = true;
        }
    }
}