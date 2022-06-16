using System;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        Shop _activeShop = null;

        public event Action activeShopChanged;

        public void SetActiveShop(Shop shop)
        {
            if (_activeShop != null)
            {
                _activeShop.SetShopper(null);
            }

            _activeShop = shop;

            if (_activeShop != null)
            {
                _activeShop.SetShopper(this);
            }

            activeShopChanged?.Invoke();
        }

        public Shop GetActiveShop()
        {
            return _activeShop;
        }
    }

}