using System.Collections.Generic;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;
using UnityEngine;

namespace New_Neo_LT.Scripts.UI
{
    public class ShopController : MonoBehaviour
    {
        [SerializeField] private GameObject itemImagePool;
        [SerializeField] private GameObject inventoryGrid;
        [SerializeField] private GameObject shopGrid;
        
        [SerializeField] private GameObject itemPrefab;
        
        private int[] _localPlayerInventory = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        
        private ShopItemIcon[] _items = new ShopItemIcon[10];
        
        private List<ShopItemIcon>      _inventory = new ();
        private List<ShopItemIcon>      _sellTable = new ();

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
            _sellTable.Clear();
        }
        
        public void SetLocalPlayerInventory(int[] inventory)
        {
            _localPlayerInventory = inventory;
        }
        
        private void AddItemToInventoryGrid(ShopItemIcon item)
        {
            _inventory.Add(item);
            item.transform.SetParent(inventoryGrid.transform);
        }
        
        private void AddItemToShopGrid(ShopItemIcon item)
        {
            _sellTable.Add(item);
            _inventory.Remove(item);
            item.transform.SetParent(shopGrid.transform);
        }
        
        private void ReturnItemToPool(ShopItemIcon item)
        {
            item.transform.SetParent(itemImagePool.transform);
            item.transform.localPosition = Vector3.zero;
        }
    }
}
