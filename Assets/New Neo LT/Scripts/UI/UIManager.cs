using System;
using System.Collections;
using System.Collections.Generic;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public enum UIType
    {
        Null = -1,
        CompassRotate,
        InventorySlotController,
        TimerController,
        PlayerListController,
        RelicPriceUI,
        ShopController,
        JobChangerUI,
        RelicScanUI,
        ReadyStateUI,
        StateLoadingUI,
        ResultUIController,
        interactionUI,
        WaitingUI,
        UITypeCount
    }
    
    public class UIManager : MonoBehaviour
    {
        public CompassNavigationUIBehavior  compassRotate;
        public InventorySlotController      inventorySlotController;
        public TimerController              timerController;
        public PlayerListController         playerListController;
        public RelicPriceUI                 relicPriceUI;
        public ShopController               shopController;
        public JobChangerUI                 jobChangerUI;
        public RelicScanUI                  RelicScanUI;
        public ReadyStateUI                 readyStateUI;
        public StateLoadingUI               stateLoadingUI;
        public ResultUIController           resultUIController;
        public InteractionUI                interactionUI;
        public WaitingUIController          waitingUIController;

        public static UIManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManager>();
                }

                return _instance;
            }
            private set => _instance = value;
        }

        private static UIManager _instance;

        private Transform _localPlayerTransform;

        private void Start()
        {
            if(Instance == null)
                Instance = this;
            else if(Instance != this)
                Destroy(gameObject);
            
            // SetActiveUI(UIType.ResultUIController, false);
        }

        // 게임 접속 시 UI 초기화
        public void InitializeInGameUI()
        {
            _localPlayerTransform = PlayerCharacter.Local.transform;
            shopController.InitShopUI();
        }

        public void EnterWaitingState()
        {
            
            SetActiveUI(UIType.WaitingUI, true);
        }
        
        public void EnterPreGameState()
        {
            SetActiveUI(UIType.WaitingUI, false);
            // SetActiveUI(UIType.ResultUIController, false);
            
            SetActiveUI(UIType.JobChangerUI, true);
        }

        public void EnterPlayState()
        {
            SetActiveUI(UIType.JobChangerUI, false);
            SetActiveUI(UIType.ResultUIController, false);
            
            
            SetActiveUI(UIType.InventorySlotController, true);
            SetActiveUI(UIType.CompassRotate, true);
            SetActiveUI(UIType.TimerController, true);
            SetActiveUI(UIType.PlayerListController, true);
            SetActiveUI(UIType.RelicPriceUI, true);
        }

        public void EnterLoadingState()
        {
            
        }
        
        public void EnterEndGameState()
        {
            
            SetActiveUI(UIType.ResultUIController, true);
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

        public void SetActiveUI(UIType type, bool isActive)
        {
            switch (type)
            {
                case UIType.CompassRotate:
                    compassRotate.gameObject.SetActive(isActive);
                    break;
                case UIType.InventorySlotController:
                    inventorySlotController.gameObject.SetActive(isActive);
                    break;
                case UIType.TimerController:
                    timerController.gameObject.SetActive(isActive);
                    break;
                case UIType.PlayerListController:
                    playerListController.gameObject.SetActive(isActive);
                    if(isActive) playerListController.InitPlayersName();
                    break;
                case UIType.RelicPriceUI:
                    relicPriceUI.gameObject.SetActive(isActive);
                    break;
                case UIType.ShopController:
                    shopController.gameObject.SetActive(isActive);
                    break;
                case UIType.JobChangerUI:
                    jobChangerUI.gameObject.SetActive(isActive);
                    break;
                case UIType.RelicScanUI:
                    RelicScanUI.gameObject.SetActive(isActive);
                    break;
                case UIType.StateLoadingUI:
                    stateLoadingUI.gameObject.SetActive(isActive);
                    break;
                case UIType.ResultUIController:
                    resultUIController.gameObject.SetActive(isActive);
                    resultUIController.Init();
                    break;
                case UIType.interactionUI:
                    interactionUI.gameObject.SetActive(isActive);
                    break;
                case UIType.WaitingUI:
                    if(!isActive)
                        StartCoroutine(nameof(FinishWaitingUI));
                    else
                        waitingUIController.gameObject.SetActive(true);
                    break;
                case UIType.Null:
                    compassRotate.gameObject.SetActive(isActive);
                    inventorySlotController.gameObject.SetActive(isActive);
                    timerController.gameObject.SetActive(isActive);
                    playerListController.gameObject.SetActive(isActive);
                    relicPriceUI.gameObject.SetActive(isActive);
                    shopController.gameObject.SetActive(isActive);
                    jobChangerUI.gameObject.SetActive(isActive);
                    RelicScanUI.gameObject.SetActive(isActive);
                    readyStateUI.gameObject.SetActive(isActive);
                    stateLoadingUI.gameObject.SetActive(isActive);
                    resultUIController.gameObject.SetActive(isActive);
                    interactionUI.gameObject.SetActive(isActive);
                    waitingUIController.gameObject.SetActive(isActive);
                    break;
                case UIType.UITypeCount:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private IEnumerator FinishWaitingUI()
        {
            waitingUIController.StartEndOfProgress();
            yield return new WaitForSeconds(2);
            
            waitingUIController.gameObject.SetActive(false);
        }
    }
}