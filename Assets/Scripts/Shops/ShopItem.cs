using GameDevTV.Inventories;
using System;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        InventoryItem item;
        int availability;
        float price;
        int quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public int GetAvailability()
        {
            return availability;
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }

        public float GetPrice()
        {
            return price;
        }

        public InventoryItem GetInventoryItem()
        {
            return this.item;
        }

        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }
    }
}
