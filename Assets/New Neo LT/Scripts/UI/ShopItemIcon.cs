using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RelicManager = LegalThieves.RelicManager;

namespace New_Neo_LT.Scripts.UI
{
    public class ShopItemIcon : MonoBehaviour, IPointerDownHandler
    {
        public int InventoryIndex => _inventoryIndex;
        public int RelicId        => _relicId;
        
        private ShopController _shopController;
        private Image          _icon;
        
        private int _inventoryIndex;
        private int _relicId;

        public void SetShopController(ShopController shopController)
        {
            _shopController = shopController;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _shopController.OnIconClick(this);
        }
        
        public void SetRelic(int inventoryIndex, int relicId)
        {
            _inventoryIndex = inventoryIndex;
            _relicId        = relicId;
            
            var typ = RelicManager.Instance.GetRelicData(relicId).GetTypeIndex();
            
            GetComponent<Image>().sprite = RelicManager.Instance.GetRelicSprite(typ);
        }
    }
}
