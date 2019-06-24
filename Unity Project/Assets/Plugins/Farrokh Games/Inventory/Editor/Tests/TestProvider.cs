using System.Collections.Generic;

namespace FarrokhGames.Inventory
{
    public class TestProvider : IInventoryProvider
    {
        private List<IInventoryItem> _items = new List<IInventoryItem>();
        private int _maximumAlowedItemCount;

        /// <summary>
        /// CTOR
        /// </summary>
        public TestProvider(InventoryRenderMode renderMode = InventoryRenderMode.Grid, int maximumAlowedItemCount = -1)
        {
            InventoryRenderMode = renderMode;
            _maximumAlowedItemCount = maximumAlowedItemCount;
        }

        public int InventoryItemCount { get { return _items.Count; } }

        public InventoryRenderMode InventoryRenderMode { get; private set; }

        public bool IsInventoryFull
        {
            get
            {
                if (_maximumAlowedItemCount < 0)return false;
                return InventoryItemCount < _maximumAlowedItemCount;
            }
        }

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