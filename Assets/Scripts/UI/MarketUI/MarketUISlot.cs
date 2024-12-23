using System;
using New_Neo_LT.Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.MarketUI
{
    public class MarketUISlot : MonoBehaviour
    {
        [Header("Slot References")]
        [SerializeField] private Image      icon;
        [SerializeField] private GameObject normalBackground;
        [SerializeField] private GameObject selectedBackground;
        
        [SerializeField] private int slotID;
        private bool _isSelected = false;
        
        public void Awake()
        {
            icon ??= GetComponentInChildren<Image>();
            normalBackground ??= transform.Find("Background_normal")?.gameObject;
            selectedBackground ??= transform.Find("Background_selected")?.gameObject;
            
            SetSelected(false);
        }
        
        public void Button_SlotClicked()
        {
            UIManager.Instance.marketUIController.Button_SlotClick(this);
            SetSelected(true);
        }
        
        public void SetSlotID(int id)
        {
            slotID = id;
            SetIcon(id);
        }
        
        public int GetSlotID()
        {
            return slotID;
        }
        
        private void SetIcon(int id)
        {
            icon.sprite = ItemManager.Instance.GetItemSprite(id);
        }
        
        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;
            selectedBackground.SetActive(isSelected);
            normalBackground.SetActive(!isSelected);
        }
    }
}