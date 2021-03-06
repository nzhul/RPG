using GameDevTV.Inventories;
using RPG.Shops;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] ItemCategory category = ItemCategory.None;

        Button button;
        Shop _currentShop;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop currentShop)
        {
            _currentShop = currentShop;
        }

        public void RefreshUI()
        {
            button.interactable = _currentShop.GetFilter() != category;
        }

        private void SelectFilter()
        {
            _currentShop.SelectFilter(category);
        }
    }
}
