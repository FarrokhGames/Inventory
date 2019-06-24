using System;
using System.Collections.Generic;

namespace FarrokhGames.Inventory.Examples
{
    public class InventoryProvider : IInventoryProvider
    {
        private List<IInventoryItem> _items = new List<IInventoryItem>();

        public int InventoryItemCount { get { return _items.Count; } }

        public bool AddInventoryItem(IInventoryItem item)
        {
            if (!_items.Contains(item))
            {
                _items.Add(item);
                return true;
            }
            return false;
        }

        public bool DropInventoryItem(IInventoryItem item)
        {
            UnityEngine.Debug.Log("Item dropped");
            return RemoveInventoryItem(item);
        }

        public IInventoryItem GetInventoryItem(int index)
        {
            return _items[index];
        }

        public bool CanAddInventoryItem(IInventoryItem item)
        {
            return true;
        }

        public bool CanRemoveInventoryItem(IInventoryItem item)
        {
            return true;
        }

        public bool CanDropInventoryItem(IInventoryItem item)
        {
            return true;
        }

        public bool RemoveInventoryItem(IInventoryItem item)
        {
            return _items.Remove(item);
        }
    }
}