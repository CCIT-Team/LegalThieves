using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MarketUI
{
    public class MarketUIController : MonoBehaviour
    {
        [Header("MarketUI References"), Header("Prefabs")]
        [SerializeField] private GameObject slotPrefab;
        [Space, Header("Page Objects")]
        [SerializeField] private GameObject itemPage;
        [SerializeField] private GameObject skillPage;
        [Space, Header("Grid group parents objects")]
        [SerializeField] private Transform  itemSlotGrid;
        [SerializeField] private Transform  skillSlotGrid;
        
        [Header("Player Info"), Header("Texts")]
        [SerializeField] private TMP_Text   playerGoldPointText;
        [SerializeField] private TMP_Text   playerRenownPointText;
        
        [Space, Header("Product Info"), Header("Images & Sprites")]
        [SerializeField] private Image      productImage;
        [Space, Header("Texts")]
        [SerializeField] private TMP_Text   productNameText;
        [SerializeField] private TMP_Text   productDescriptionText;
        [SerializeField] private TMP_Text   productPriceText;
        [SerializeField] private TMP_Text   productCountText;
        [Space, Header("Price Type Icons")]
        [SerializeField] private GameObject productPriceTypeIconRenown;
        [SerializeField] private GameObject productPriceTypeIconGold;
        [SerializeField] private GameObject productCountParent;
        
        private int CurrentProductID => _currentSlot.GetSlotID();
        
        private MarketUISlot _currentSlot;
        private int _currentProductCount = 1;


        private readonly List<MarketUISlot> _itemSlots = new();
        private readonly List<MarketUISlot> _skillSlots = new();

        public void Awake()
        {
            // InitUI();
        }

        #region Init methods...
        
        public void InitUI()
        {
            // 슬롯 초기화

            itemSlotGrid  ??= productPriceTypeIconRenown.transform.Find("Item_slot_grid")?.transform;
            skillSlotGrid ??= productPriceTypeIconGold.transform.Find("Skill_slot_grid")?.transform;
            
            var items = ItemManager.Instance.ItemBases;
            // var skills = SkillManager.Instance.skills;

            foreach (var itemBase in items)
            {
                var slot = Instantiate(slotPrefab, itemSlotGrid).GetComponent<MarketUISlot>();
                slot.SetSlotID(itemBase.ID);
                _itemSlots.Add(slot);
            }
            
            // for (var i = 0; i < skills.Length; i++)
            // {
            //     var slot = Instantiate(slotPrefab, skillSlotGrid).GetComponent<MarketUISlot>();
            //     slot.SetSlotID(i);
            //     slot.SetIcon(skills[i].skillIcon);
            //     _skillSlots[i] = slot;
            // }
        }
        
        public void OnMarketUIOpen()
        {
            Button_ShowItemPage();
        }
        
        private void InitItemPage()
        {
            var firstItemSlot = _itemSlots.FirstOrDefault();
            Button_SlotClick(firstItemSlot);
        }
        
        private void InitSkillPage()
        {
            // Button_SlotClick(_skillSlots[0].ObjectID);
        }

        #endregion
        
        
        #region Market interaction methods...
        
        public void Button_ShowItemPage()
        {
            itemPage.SetActive(true);
            skillPage.SetActive(false);
            InitItemPage();
        }
        
        public void Button_ShowSkillPage()
        {
            itemPage.SetActive(false);
            skillPage.SetActive(true);
            InitSkillPage();
        }

        public void Button_ProductCountUp()
        {
            _currentProductCount++;
            productCountText.text = _currentProductCount.ToString();
        }
        
        public void Button_ProductCountDown()
        {
            if (_currentProductCount > 1)
            {
                _currentProductCount--;
                productCountText.text = _currentProductCount.ToString();
            }
        }
        
        public void Button_SlotClick(MarketUISlot slot)
        {
            _currentSlot?.SetSelected(false);
            _currentSlot = slot;
            UpdateProductInfo();
            ResetProductCount();
        }

        public void Button_TryBuyProduct()
        {
            var pc = PlayerCharacter.Local;
            // 구매 가능한지 확인
            var price = ItemManager.Instance.GetItemPrice(CurrentProductID);
            var goldPoint = pc.GetGoldPoint;
            var index = Array.FindIndex(pc.GetItemSkillInventory(), i => i == -1);
            if (price < goldPoint && index != -1)
            {
                RPC_BuyObject(pc.Ref, index, CurrentProductID);
            }
            // 구매 불가능한 경우
            else
            {
                // 구매 실패 시 처리
            }
        }

        #endregion
        
        public void UpdatePlayerInfo(int goldPoint, int renownPoint)
        {
            playerGoldPointText.text   = goldPoint.ToString();
            playerRenownPointText.text = renownPoint.ToString();
        }
        
        public void UpdatePlayerInfo()
        {
            var goldPoint = PlayerCharacter.Local.GetGoldPoint;
            var renownPoint = PlayerCharacter.Local.GetRenownPoint;
            
            playerGoldPointText.text   = goldPoint.ToString();
            playerRenownPointText.text = renownPoint.ToString();
        }
        

        private void UpdateProductInfo()
        {
            var itemName         = ItemManager.Instance.GetItemName(CurrentProductID);
            var itemSprite       = ItemManager.Instance.GetItemSprite(CurrentProductID);
            var itemDescription  = ItemManager.Instance.GetItemDescription(CurrentProductID);
            var itemPrice     = ItemManager.Instance.GetItemPrice(CurrentProductID);
            
            productNameText.text         = itemName;
            productImage.sprite          = itemSprite;
            productDescriptionText.text  = itemDescription;
            productPriceText.text        = itemPrice.ToString();
            
            SetProductPriceType(skillPage.activeSelf);
            ResetProductCount();
        }

        private void ResetProductCount()
        {
            _currentProductCount = 1;
            productCountText.text = _currentProductCount.ToString();
        }
        
        private void SetProductPriceType(bool isRenown)
        {
            productPriceTypeIconRenown.SetActive(isRenown);
            productPriceTypeIconGold.SetActive(!isRenown);
        }
        
        // 여러개 구매 가능 여부
        private void CheckMultipleBuyable()
        {
            if (true)
            {
                productCountParent.SetActive(true);
            }
            else
            {
                // productCountParent.SetActive(false);
            }
        }

        #region RPC methods...

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
        private static void RPC_BuyObject(PlayerRef player, int index, int objectID)
        {
            var playerCharacter = PlayerRegistry.GetPlayer(player);
            
            playerCharacter.SetItemSkillInventory(index, objectID);
            playerCharacter.AddGoldPoint(-ItemManager.Instance.GetItemPrice(objectID));
        }

        #endregion
    }
}