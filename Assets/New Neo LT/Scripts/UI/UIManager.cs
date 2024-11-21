using New_Neo_LT.Scripts.PlayerComponent;
using System.Collections;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        public CompassRotate           compassRotate;
        public InventorySlotController inventorySlotController;
        public TimerController         timerController;
        public PlayerListController    playerListController;
        public RelicPriceUI            relicPriceUI;
        public ShopController          shopController;
        public JobChangerUI            jobChangerUI;

        public static UIManager Instance;

        [Header("Inventory UI")]
        [SerializeField] private RectTransform inventoryUI; // 인벤토리 UI RectTransform
        [SerializeField] private Vector3 hiddenPosition = new Vector3(0, -900, 0); // 화면 밖 위치
        [SerializeField] private Vector3 visiblePosition = new Vector3(0, -370, 0); // 화면에 보이는 위치
        private bool isInventoryOpen = false; // 인벤토리 열림 상태
        private bool isCooldownActive = false; // 쿨다운 상태

        private Transform _localPlayerTransform;

        private void Start()
        {
            if(Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
        }

        private void LateUpdate()
        {
            // 서버 접속 확인
            if (!_localPlayerTransform)
                return;
            
            compassRotate.RotateCompass(_localPlayerTransform);

        }

        // 게임 접속 시 UI 초기화
        public void InitializeInGameUI()
        {
            _localPlayerTransform = PlayerCharacter.Local.transform;
            shopController.InitShopUI();
        }
        
        public Transform GetLocalPlayerTransform()
        {
            return _localPlayerTransform;
        }

        public void OpenShop()
        {
            shopController.gameObject.SetActive(true);
            shopController.OnShopOpen();
        }
        
        public void CloseShop()
        {
            shopController.gameObject.SetActive(false);
            shopController.OnShopClose();
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

            yield return new WaitForSeconds(0.3f);
            isCooldownActive = false; // 쿨다운 해제   
        }
    }
}