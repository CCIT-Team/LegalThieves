﻿using System;
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

        private void LateUpdate()
        {
            // 서버 접속 확인
            if (!_localPlayerTransform)
                return;
            
            //compassRotate.RotateCompass(_localPlayerTransform);
            
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
                case UIType.Null:
                    // If isActive is false then all UIs are set to false
                    if(isActive)
                        break;
                    resultUIController.gameObject.SetActive(false);
                    stateLoadingUI.gameObject.SetActive(false);
                    RelicScanUI.gameObject.SetActive(false);
                    jobChangerUI.gameObject.SetActive(false);
                    shopController.gameObject.SetActive(false);
                    relicPriceUI.gameObject.SetActive(false);
                    playerListController.gameObject.SetActive(false);
                    timerController.gameObject.SetActive(false);
                    inventorySlotController.gameObject.SetActive(false);
                    compassRotate.gameObject.SetActive(false);
                    interactionUI.gameObject.SetActive(false);
                    break;
                case UIType.UITypeCount:
                    // Do nothing
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}