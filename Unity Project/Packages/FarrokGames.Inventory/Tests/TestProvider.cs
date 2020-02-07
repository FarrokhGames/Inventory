using System.Collections.Generic;

namespace FarrokhGames.Inventory
{
    public class TestProvider : IInventoryProvider
    {
        private readonly List<IInventoryItem> _items = new List<IInventoryItem>();
        private readonly int _maximumAlowedItemCount;

        /// <summary>
        /// CTOR
        /// </summary>
        public TestProvider(InventoryRenderMode renderMode = InventoryRenderMode.Grid, int maximumAlowedItemCount = -1)
        {
            inventoryRenderMode = renderMode;
            _maximumAlowedItemCount = maximumAlowedItemCount;
        }

        public int inventoryItemCount => _items.Count;

        public InventoryRenderMode inventoryRenderMode { get; }

        public bool isInventoryFull
        {
            get
            {
                if (_maximumAlowedItemCount < 0) return false;
                return inventoryItemCount < _maximumAlowedItemCount;
            }
        }

        public bool AddInventoryItem(IInventoryItem item)
        {
            if (_items.Contains(item)) return false;
            _items.Add(item);
            return true;
        }

        public bool DropInventoryItem(IInventoryItem item) => RemoveInventoryItem(item);
        public IInventoryItem GetInventoryItem(int index) => _items[index];
        public bool CanAddInventoryItem(IInventoryItem item) => true;
        public bool CanRemoveInventoryItem(IInventoryItem item) => true;
        public bool CanDropInventoryItem(IInventoryItem item) => true;
        public bool RemoveInventoryItem(IInventoryItem item) => _items.Remove(item);
    }
}