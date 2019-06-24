using System.Linq;
using FarrokhGames.Inventory.Internal;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Inventory Class
    /// </summary>
    public class InventoryManager : AbstractInventoryManager
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="provider">The IInventoryProvider to use with this inventory</param>
        /// <param name="width">Width of this inventory</param>
        /// <param name="height">Height of this inventory</param>
        public InventoryManager(IInventoryProvider provider, int width, int height) : base(provider)
        {
            SetSize(width, height);
        }

        /// <summary>
        /// Sets the size of the inventory, dropping any item that 
        /// no longer fits
        /// </summary>
        public void SetSize(int width, int height)
        {
            Resize(width, height);
        }

        /// <inheritdoc />
        public override bool IsFull
        {
            get
            {
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        if (GetAtPoint(new Vector2Int(x, y)) == null) { return false; }
                    }
                }
                return true;
            }
        }

        /// <inheritdoc />
        public override bool CanAdd(IInventoryItem item)
        {
            if (AllItems.Contains(item))return false;
            Vector2Int point;
            if (GetFirstPointThatFitsItem(item, out point))
            {
                return CanAddAt(item, point);
            }
            return false;
        }

        /// <inheritdoc />
        public override bool Add(IInventoryItem item)
        {
            if (!CanAdd(item))return false;
            Vector2Int point;
            if (GetFirstPointThatFitsItem(item, out point))
            {
                return AddAt(item, point);
            }
            return false;
        }

        /// <inheritdoc />
        public override bool CanAddAt(IInventoryItem item, Vector2Int point)
        {
            var previousPoint = item.Position;
            item.Position = point;
            var padding = Vector2.one * 0.01f;

            // Check if item is outside of inventory
            if (!Rect.Contains(item.GetMinPoint() + padding) || !Rect.Contains(item.GetMaxPoint() - padding))
            {
                item.Position = previousPoint;
                return false;
            }

            // Check if item overlaps another item already in the inventory
            foreach (var otherItem in AllItems)
            {
                if (item.Overlaps(otherItem))
                {
                    item.Position = previousPoint;
                    return false;
                }
            }

            return true; // Item can be added
        }

        /*
         * Get first free point that will fit the given item
         */
        private bool GetFirstPointThatFitsItem(IInventoryItem item, out Vector2Int point)
        {
            for (var x = 0; x < Width - (item.Width - 1); x++)
            {
                for (var y = 0; y < Height - (item.Height - 1); y++)
                {
                    point = new Vector2Int(x, y);
                    if (CanAddAt(item, point))return true;
                }
            }
            point = Vector2Int.zero;
            return false;
        }

        /// <inheritdoc />
        protected override void HandleSizeChanged()
        {
            // Drop all items that no longer fit the inventory
            for (int i = 0; i < AllItems.Length;)
            {
                var item = AllItems[i];
                var shouldBeDropped = false;
                var padding = Vector2.one * 0.01f;

                if (!Rect.Contains(item.GetMinPoint() + padding) || !Rect.Contains(item.GetMaxPoint() - padding))
                {
                    shouldBeDropped = true;
                }

                if (shouldBeDropped)
                {
                    Drop(item);
                }
                else
                {
                    i++;
                }
            }
        }
    }
}