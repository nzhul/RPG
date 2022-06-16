using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image iconField;
        [SerializeField] TextMeshProUGUI nameField;
        [SerializeField] TextMeshProUGUI availabilityField;
        [SerializeField] TextMeshProUGUI priceField;
        [SerializeField] TextMeshProUGUI quantityField;

        Shop _currentShop = null;
        ShopItem _item = null;

        public void Setup(Shop _currentShop, ShopItem item)
        {
            this._currentShop = _currentShop;
            this._item = item;
            iconField.sprite = item.GetIcon();
            nameField.text = item.GetName();
            availabilityField.text = $"{item.GetAvailability()}";
            priceField.text = $"${item.GetPrice():N2}";
            quantityField.text = $"{item.GetQuantityInTransaction()}";
        }

        public void Add()
        {
            _currentShop.AddToTransaction(_item.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            _currentShop.AddToTransaction(_item.GetInventoryItem(), -1);
        }
    }
}
