using System;
using LegalThieves;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
namespace New_Neo_LT.Scripts.UI
{
    public class InventorySlotController : MonoBehaviour
    {
        [SerializeField] private GameObject[] slots;
        private int _prevIndex;
        public int CurrentIndex => _prevIndex;

        [Header("Inventory UI")]
        [SerializeField] private RectTransform inventoryUI; // 인벤토리 UI RectTransform
        [SerializeField] private Vector3 hiddenPosition;// 화면 밖 위치
        [SerializeField] private Vector3 visiblePosition;
        private bool isInventoryOpen = true; // 인벤토리 열림 상태
        private bool isCooldownActive = false; // 쿨다운 상태
        WaitForSeconds  wait_0_3=  new WaitForSeconds(0.1f);
        private int inventoryonoff;
        public int inventoryclose => inventoryonoff;


        private void Start()
        {
            visiblePosition = inventoryUI.GetComponent<RectTransform>().anchoredPosition ;
            hiddenPosition = visiblePosition + Vector3.left * 500;
            SelectToggle(0);
            inventoryonoff = 1;

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
        public void OnToggleInventory()
        {
            if (!isCooldownActive) // 쿨다운 중이 아니면 동작
            {
                isCooldownActive = true; // 쿨다운 시작
                StartCoroutine(ToggleInventoryCoroutine());
            }
        }

        private IEnumerator ToggleInventoryCoroutine()
        {
            isInventoryOpen = !isInventoryOpen; // 상태 반전
            inventoryUI.anchoredPosition = isInventoryOpen ? visiblePosition : hiddenPosition; // UI 위치 변경
            Debug.Log($"Inventory is now {(isInventoryOpen ? "open" : "closed")}.");

            if (isInventoryOpen == false)
            {
                inventoryonoff = 0;
            }
            else if (isInventoryOpen == true)
            {
                inventoryonoff = 1;
            }

            yield return wait_0_3;
            isCooldownActive = false; // 쿨다운 해제   
        }
        public void MoveCurrentSlot(bool isLeft)
        {
            switch (isLeft)
            {
                case true when _prevIndex > 0:
                    slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(false);
                    _prevIndex -= 1;
                    slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(true);
                    break;
                case false when _prevIndex < 9:
                    slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(false);
                    _prevIndex += 1;
                    slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(true);
                    break;
            }
        }

    }
}