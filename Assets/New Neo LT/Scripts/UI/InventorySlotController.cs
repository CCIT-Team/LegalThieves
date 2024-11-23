using System;
using LegalThieves;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace New_Neo_LT.Scripts.UI
{
    public class InventorySlotController : MonoBehaviour
    {
        [SerializeField] private GameObject[] slots;
        [SerializeField] private GameObject selectSlot;
                         private TMP_Text   goldPoint;
                         private TMP_Text   renownPoint;

        private int _prevIndex;
        public int CurrentIndex => _prevIndex;
        private int inventoryonoff;
        public int inventoryclose => inventoryonoff;


        [Header("Inventory UI")]
        [SerializeField] private RectTransform inventorybagopenslottUI;
        [SerializeField] private RectTransform inventorybagcloseslottUI;
        [SerializeField] private RectTransform inventoryUI; // 인벤토리 UI RectTransform
        [SerializeField] private Vector3 hiddenPosition;// 화면 밖 위치
        [SerializeField] private Vector3 visiblePosition;
        private bool isInventoryOpen = true; // 인벤토리 열림 상태
        private bool isCooldownActive = false; // 쿨다운 상태
        WaitForSeconds  wait_0_3=  new WaitForSeconds(0.1f);

        private void Start()
        {
            visiblePosition = inventoryUI.GetComponent<RectTransform>().anchoredPosition ;
            hiddenPosition = visiblePosition + Vector3.left * 500;
            goldPoint = selectSlot.transform.GetChild(4).GetComponent<TMP_Text>();
            renownPoint = selectSlot.transform.GetChild(5).GetComponent<TMP_Text>();
            SelectToggle(0);
        }

        public void SelectToggle(int index)
        {
            //slots[_prevIndex].transform.GetChild(1).gameObject.SetActive(false);
            selectSlot.transform.SetParent(slots[index].transform);
            selectSlot.transform.localPosition = Vector3.zero;
            
            //slots[index].transform.GetChild(1).gameObject.SetActive(true);
            _prevIndex = index;
        }
        
        public void SetSlotPoint(int rindex)
        {
            if (rindex == -1)
            {
                selectSlot.transform.GetChild(2).gameObject.SetActive(false);
                selectSlot.transform.GetChild(3).gameObject.SetActive(false);
                goldPoint.text = "";
                renownPoint.text = "";
            }
            else
            {
                var relic = RelicManager.Instance.GetRelicData(rindex);
                selectSlot.transform.GetChild(2).gameObject.SetActive(true);
                selectSlot.transform.GetChild(3).gameObject.SetActive(true);
                goldPoint.text = relic.GetGoldPoint().ToString();
                renownPoint.text = relic.GetRenownPoint().ToString();
            }
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
            Vector3 startPosition = inventoryUI.anchoredPosition; // 시작 위치
            Vector3 targetPosition = isInventoryOpen ? visiblePosition : hiddenPosition; // 목표 위치

            float duration = 0.3f; // 애니메이션 지속 시간
            float elapsedTime = 0f; // 경과 시간

            Debug.Log($"Inventory is now {(isInventoryOpen ? "open" : "closed")}.");


            if (isInventoryOpen == false)
            {
                inventoryonoff = 0;
            }
            else if (isInventoryOpen == true)
            {
                inventoryonoff = 1;
            }


            // 애니메이션 루프
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime; // 프레임 간 시간 추가
                float t = Mathf.Clamp01(elapsedTime / duration); // 0~1 사이로 정규화
                inventoryUI.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, t); // 위치 보간
                yield return null; // 다음 프레임까지 대기
            }

            // 최종 위치 보정
            inventoryUI.anchoredPosition = targetPosition;
            {
                inventorybagopenslottUI.gameObject.SetActive(true);
                inventorybagcloseslottUI.gameObject.SetActive(false);
            }
            if (isInventoryOpen == false)
            {
                inventorybagopenslottUI.gameObject.SetActive(false);
                inventorybagcloseslottUI.gameObject.SetActive(true);
            }

            yield return wait_0_3;
            isCooldownActive = false; // 쿨다운 해제   
        }

    }
}