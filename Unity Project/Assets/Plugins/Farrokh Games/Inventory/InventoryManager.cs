using System;
using System.Collections.Generic;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Inventory Class
    /// </summary>
    public class InventoryManager : IInventoryManager
    {
        /// <summary>
        /// Invoked when an item is added to the inventory
        /// </summary>
        public Action<IInventoryItem> OnItemAdded { get; set; }

        /// <summary>
        /// Invoked when an item is removed to the inventory
        /// </summary>
        public Action<IInventoryItem> OnItemRemoved { get; set; }

        /// <summary>
        /// Invoked when an item is removed from the inventory and should be placed on the ground.
        /// </summary>
        public Action<IInventoryItem> OnItemDropped { get; set; }

        /// <summary>
        /// Invoked when the inventory is cleared.
        /// </summary>
        public Action OnCleared { get; set; }

        /// <summary>
        /// Invoked when the inventory is resized.
        /// </summary>
        public Action OnResized { get; set; }

        /// <inheritdoc />
        public int Width { get; private set; }

        /// <inheritdoc />
        public int Height { get; private set; }

        private List<IInventoryItem> _items = new List<IInventoryItem>();
        private Rect _fullRect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">Width of this inventory</param>
        /// <param name="height">Height of this inventory</param>
        public InventoryManager(int width, int height)
        {
            SetSize(width, height);
        }

        /*
        Sets the width and height of the inventory and recalculates the _fullRect
        */
        private void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            _fullRect = new Rect(0, 0, Width, Height);
        }

        /// <summary>
        /// Returns a list of all items within this inventory
        /// </summary>
        public List<IInventoryItem> AllItems { get { return new List<IInventoryItem>(_items); } }

        /// <inheritdoc />
        public IInventoryItem[] GetAllItems()
        {
            return _items.ToArray();
        }

        /// <inheritdoc />
        public bool Contains(IInventoryItem item) { return _items.Contains(item); }

        /// <inheritdoc />
        public bool IsFull
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
        public bool CanAdd(IInventoryItem item)
        {
            if (_items.Contains(item))return false;
            return GetFirstPointThatFitsItem(item).x != -1;
        }

        /// <inheritdoc />
        public bool Add(IInventoryItem item)
        {
            if (!CanAdd(item))return false;
            var freePoint = GetFirstPointThatFitsItem(item);
            if (freePoint.x == -1)return false;
            return AddAt(item, freePoint);
        }

        /// <inheritdoc />
        public bool CanAddAt(IInventoryItem item, Vector2Int point)
        {
            var previousPoint = item.Shape.Position;
            item.Shape.Position = point;
            var padding = Vector2.one * 0.01f;

            // Check if item is outside of inventory
            if (!_fullRect.Contains(item.Shape.Rect.min + padding) || !_fullRect.Contains(item.Shape.Rect.max - padding))
            {
                item.Shape.Position = previousPoint;
                return false;
            }

            // Check if item overlaps another item already in the inventory
            foreach (var i in _items)
            {
                if (item.Shape.Overlaps(i.Shape))
                {
                    item.Shape.Position = previousPoint;
                    return false;
                }
            }

            return true; // Item can be added
        }

        /// <inheritdoc />
        public bool AddAt(IInventoryItem item, Vector2Int Point)
        {
            if (!CanAdd(item))return false;
            if (CanAddAt(item, Point))
            {
                _items.Add(item);
                item.Shape.Position = Point;
                if (OnItemAdded != null) { OnItemAdded(item); }
            }

            return false; // TODO:
        }

        /// <inheritdoc />
        public bool CanRemove(IInventoryItem item)
        {
            return Contains(item);
        }

        /// <inheritdoc />
        public bool Remove(IInventoryItem item)
        {
            if (CanRemove(item))
            {
                _items.Remove(item);
                if (OnItemRemoved != null) { OnItemRemoved(item); }
            }

            return false; // TODO:
        }

        /// <inheritdoc />
        public bool CanDrop(IInventoryItem item)
        {
            return CanRemove(item);
        }

        /// <inheritdoc />
        public bool Drop(IInventoryItem item)
        {
            Remove(item);
            if (OnItemDropped != null) { OnItemDropped(item); }
            return false; // TODO:
        }

        /// <inheritdoc />
        public void DropAll()
        {
            var itemsToDrop = _items.ToArray();
            foreach (var item in itemsToDrop)
            {
                Drop(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _items.Clear();
            if (OnCleared != null) { OnCleared(); }
        }

        /// <summary>
        /// Get an item at given point within this inventory
        /// </summary>
        /// <param name="point">Point at which to look for item</param>
        public IInventoryItem GetAtPoint(Vector2Int point)
        {
            foreach (var item in _items)
            {
                if (item.Shape.Contains(point)) { return item; }
            }
            return null;
        }

        /// <summary>
        /// Resize the inventory
        /// Items that no longer fit will be dropped.
        /// </summary>
        /// <param name="newWidth">The new width</param>
        /// <param name="newHeight">The new height</param>
        public void Resize(int newWidth, int newHeight)
        {
            SetSize(newWidth, newHeight);

            // Drop all items that no longer fit the inventory
            for (int i = 0; i < _items.Count;)
            {
                var item = _items[i];
                var shouldBeDropped = false;
                for (int j = 0; j < item.Shape.Points.Length; j++)
                {
                    if (!_fullRect.Contains(item.Shape.Points[j]))
                    {
                        shouldBeDropped = true;
                        break;
                    }
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

            if (OnResized != null) { OnResized(); }
        }

        /*
         * Get first free point that will fit the given item
         */
        private Vector2Int GetFirstPointThatFitsItem(IInventoryItem item)
        {
            for (var x = 0; x < Width - (item.Shape.Width - 1); x++)
            {
                for (var y = 0; y < Height - (item.Shape.Height - 1); y++)
                {
                    var p = new Vector2Int(x, y);
                    if (CanAddAt(item, p))return p;
                }
            }
            return new Vector2Int(-1, -1);
        }
    }
}