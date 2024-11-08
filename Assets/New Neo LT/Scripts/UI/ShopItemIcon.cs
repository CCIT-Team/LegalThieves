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

        private void Start()
        {
            _icon = GetComponent<Image>();
        }

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
            
            Debug.Log(typ);

            if (RelicManager.Instance == null)
            {
                Debug.Log("tq");
            }
            
            
            
            _icon.sprite = RelicManager.Instance.GetRelicSprite(typ);
        }
    }
}
