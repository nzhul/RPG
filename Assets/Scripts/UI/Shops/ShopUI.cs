using RPG.Shops;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI shopName;
        [SerializeField] Transform listRoot;
        [SerializeField] RowUI rowPrefab;
        [SerializeField] TextMeshProUGUI totalField;
        [SerializeField] Button confirmButton;
        [SerializeField] Button switchButton;

        Shopper _shopper = null;
        Shop _currentShop = null;

        Color originalTotalTextColor;

        private void Start()
        {
            originalTotalTextColor = totalField.color;

            _shopper = GameObject.FindGameObjectWithTag("Player").GetComponent<Shopper>();
            if (_shopper == null) return;

            _shopper.activeShopChanged += ShopChanged;
            confirmButton.onClick.AddListener(ConfirmTransaction);
            switchButton.onClick.AddListener(SwitchMode);

            ShopChanged();
        }

        private void ShopChanged()
        {
            if (_currentShop != null)
            {
                _currentShop.onChange -= RefreshUI;
            }

            _currentShop = _shopper.GetActiveShop();
            gameObject.SetActive(_currentShop != null);

            foreach (var button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.SetShop(_currentShop);
            }

            if (_currentShop == null) return;
            shopName.text = _currentShop.GetShopName();

            _currentShop.onChange += RefreshUI;

            RefreshUI();
        }

        private void RefreshUI()
        {
            foreach (Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach (var item in _currentShop.GetFilteredItems())
            {
                var row = Instantiate(rowPrefab, listRoot);
                row.Setup(_currentShop, item);
            }

            totalField.text = $"Total: ${_currentShop.TransactionTotal():N2}";
            totalField.color = _currentShop.HasSufficientFunds() ? originalTotalTextColor : Color.red;
            confirmButton.interactable = _currentShop.CanTransact();
            var switchText = switchButton.GetComponentInChildren<TextMeshProUGUI>();
            var confirmText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();

            if (_currentShop.IsBuyingMode())
            {
                switchText.text = "Switch To Selling";
                confirmText.text = "Buy";
            }
            else
            {
                switchText.text = "Switch to Buying";
                confirmText.text = "Sell";
            }

            foreach (var button in GetComponentsInChildren<FilterButtonUI>())
            {
                button.RefreshUI();
            }
        }

        public void Close()
        {
            _shopper.SetActiveShop(null);
        }

        public void ConfirmTransaction()
        {
            _currentShop.ConfirmTransaction();
        }

        public void SwitchMode()
        {
            _currentShop.SelectMode(!_currentShop.IsBuyingMode());
        }
    }

}