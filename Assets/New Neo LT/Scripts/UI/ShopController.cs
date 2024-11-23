using System.Collections.Generic;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;
using TMPro;

namespace New_Neo_LT.Scripts.UI
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private GameObject itemImagePool;
        [SerializeField] private GameObject inventoryGrid;
        [SerializeField] private GameObject shopGrid;
        [SerializeField] private TMP_Text   goldPointText;
        [SerializeField] private TMP_Text   renownPointText;

        [SerializeField] private GameObject itemPrefab;
        
        private int[] _localPlayerInventory = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        
        private ShopItemIcon[] _items = new ShopItemIcon[10];
        
        private List<ShopItemIcon>      _inventory = new ();
        private List<ShopItemIcon>      _sellTable = new ();

        private int goldPoint = 0;
        private int renownPoint = 0;

        public void InitShopUI()
        {
            for (var i = 0; i < _items.Length; i++)
            {
                var item = Instantiate(itemPrefab).GetComponent<ShopItemIcon>();
                item.SetShopController(this);
                _items[i] = item;
                ReturnItemToPool(item);
            }
            OnShopOpen();
            OnShopClose();
            gameObject.SetActive(false);
        }
        
        public void OnShopOpen()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UIManager.Instance?.inventorySlotController.gameObject.SetActive(false);
            
            var playerInventory = _localPlayerInventory;
            
            for (var i = 0; i < 10; i++)
            {
                var rId = playerInventory[i];
                if(rId == -1)
                    continue;
                
                _items[i].SetRelic(i, rId);
                AddItemToInventoryGrid(_items[i]);
            }

            // 플레이어 이동 인풋 제한
            NewGameManager.State.SetUIFlag(false);
        }
        
        public void OnShopClose()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            UIManager.Instance.inventorySlotController.gameObject.SetActive(true);
            
            for (var i = 0; i < 10; i++)
            {
                ReturnItemToPool(_items[i]);
            }
            _sellTable.Clear();
            
            NewGameManager.State.SetUIFlag(true);
        }

        public void OnIconClick(ShopItemIcon item)
        {
            if (item.transform.parent == inventoryGrid.transform)
            {
                AddItemToShopGrid(item);
            }
            else
            {
                AddItemToInventoryGrid(item);
                _sellTable.Remove(item);
            }
        }
        
        public void OnSellButtonClick()
        {
            if(_sellTable.Count == 0)
                return;

            var playerRef = PlayerCharacter.Local.Ref;
            var sellTable = new int[_sellTable.Count];
            
            for (var i = 0; i < _sellTable.Count; i++)
            {
                var item = _sellTable[i];
                sellTable[i] = item.InventoryIndex;
                ReturnItemToPool(item);
            }
            
            NewGameManager.Instance.RPC_SellRelics(playerRef, sellTable);

            goldPointText.text = "0";
            renownPointText.text = "0";

            _sellTable.Clear();
        }
        
        public void SetLocalPlayerInventory(int[] inventory)
        {
            _localPlayerInventory = inventory;
        }
        
        private void AddItemToInventoryGrid(ShopItemIcon item)
        {
            _inventory.Add(item);

            goldPoint -= LegalThieves.RelicManager.Instance.GetRelicData(item.RelicId).GetGoldPoint();
            renownPoint -= LegalThieves.RelicManager.Instance.GetRelicData(item.RelicId).GetRenownPoint();

            goldPointText.text = Mathf.Max(goldPoint,0).ToString();
            renownPointText.text = Mathf.Max(renownPoint, 0).ToString();

            item.transform.SetParent(inventoryGrid.transform);
        }
        
        private void AddItemToShopGrid(ShopItemIcon item)
        {
            _sellTable.Add(item);
            _inventory.Remove(item);

            goldPoint += LegalThieves.RelicManager.Instance.GetRelicData(item.RelicId).GetGoldPoint();
            renownPoint += LegalThieves.RelicManager.Instance.GetRelicData(item.RelicId).GetRenownPoint();

            goldPointText.text = goldPoint.ToString();
            renownPointText.text = renownPoint.ToString();

            item.transform.SetParent(shopGrid.transform);
        }
        
        private void ReturnItemToPool(ShopItemIcon item)
        {
            item.transform.SetParent(itemImagePool.transform);
            item.transform.localPosition = Vector3.zero;
        }
    }
}
