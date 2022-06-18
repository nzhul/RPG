using GameDevTV.Inventories;
using RPG.Control;
using RPG.Inventories;
using RPG.Saving;
using RPG.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName;

        [Range(0f, 100f)]
        [SerializeField] float sellingPercentage = 80f;
        [SerializeField] StockItemConfig[] stockConfig;

        [Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0f, 100f)]
            public float buyingDiscountPercentage;
            public int levelToUnlock = 0;

        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        private Shopper _currentShopper;
        private bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;

        public event Action onChange;

        private void Awake()
        {
            foreach (var config in stockConfig)
            {
                stockSold[config.item] = config.initialStock;
            }
        }

        public void SetShopper(Shopper shopper)
        {
            this._currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (var shopItem in GetAllItems())
            {
                var item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            var prices = GetPrices();
            var availabilities = GetAvailabilities();

            foreach (var item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) continue;


                float price = prices[item];
                transaction.TryGetValue(item, out int quantityInTransaction);
                var availability = availabilities[item];
                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;
            print(category);
            onChange?.Invoke();
        }

        public ItemCategory GetFilter()
        {
            return filter;
        }

        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;
            onChange?.Invoke();
        }

        public bool IsBuyingMode()
        {
            return isBuyingMode;
        }

        public bool CanTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasInventorySpace()) return false;
            return true;
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) return true;

            var purse = _currentShopper.GetComponent<Purse>();
            if (purse == null) return false;

            return purse.GetBalance() >= TransactionTotal();
        }

        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }

        public bool HasInventorySpace()
        {
            if (!isBuyingMode) return true;

            var shopperInventory = _currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) return false;

            var flatItems = new List<InventoryItem>();
            foreach (var shopItem in GetAllItems())
            {
                var item = shopItem.GetInventoryItem();
                var quantity = shopItem.GetQuantityInTransaction();

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }

        internal string GetShopName()
        {
            return shopName;
        }

        public void ConfirmTransaction()
        {
            var shopperInventory = _currentShopper.GetComponent<Inventory>();
            var shopperPurse = _currentShopper.GetComponent<Purse>();
            if (shopperInventory == null || shopperPurse == null) return;

            foreach (var shopItem in GetAllItems())
            {
                var item = shopItem.GetInventoryItem();
                var quantity = shopItem.GetQuantityInTransaction();
                var price = shopItem.GetPrice();

                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }

            onChange?.Invoke();
        }

        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            var slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) return;

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            stockSold[item]++;
            shopperPurse.UpdateBalance(price);

        }

        public float TransactionTotal()
        {
            float total = 0;

            foreach (var item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }

            return total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            var availablity = GetAvailability(item);
            if (transaction[item] + quantity > availablity)
            {
                transaction[item] = availablity;
            }
            else
            {
                transaction[item] += quantity;

            }

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            onChange?.Invoke();
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }


        private int GetAvailability(InventoryItem item)
        {
            if (isBuyingMode)
            {
                return stockSold[item];
            }

            return CountItemsInInventory(item);
        }

        private int CountItemsInInventory(InventoryItem item)
        {
            var inventory = _currentShopper.GetComponent<Inventory>();

            if (inventory == null) return 0;

            int total = 0;
            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (inventory.GetItemInSlot(i) == item)
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }

            return total;
        }

        private float GetPrice(StockItemConfig config)
        {
            if (isBuyingMode)
            {
                return config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100);
            }

            return config.item.GetPrice() * (sellingPercentage / 100);
        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> result = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (!result.ContainsKey(config.item))
                {
                    result[config.item] = 0;
                }

                result[config.item] += config.initialStock;
            }

            return result;
        }

        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> result = new Dictionary<InventoryItem, float>();

            foreach (var config in stockConfig)
            {
                if (!result.ContainsKey(config.item))
                {
                    result[config.item] = config.item.GetPrice();
                }

                result[config.item] *= (1 - config.buyingDiscountPercentage / 100);
            }

            return result;
        }

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            var shopperLevel = GetShopperLevel();

            foreach (var config in stockConfig)
            {
                if (config.levelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }

        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) return;

            var success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                stockSold[item]--;
                shopperPurse.UpdateBalance(-price);
            }
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }

            return -1;
        }

        private int GetShopperLevel()
        {
            var stats = _currentShopper.GetComponent<BaseStats>();
            if (stats == null) return 0;

            return stats.GetLevel();
        }

        public object CaptureState()
        {
            var saveObject = new Dictionary<string, int>();
            foreach (var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            var saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach (var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }

}
