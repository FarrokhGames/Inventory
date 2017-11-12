using System;
using System.Collections.Generic;
using FarrokhGames.Shared;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Inventory Class
    /// </summary>
    public class InventoryManager
    {
        /// <summary>
        /// Invoked when an item is added to the inventory
        /// </summary>
        public Action<IInventoryItem> OnItemAdded;

        /// <summary>
        /// Invoked when an item is removed to the inventory
        /// </summary>
        public Action<IInventoryItem> OnItemRemoved;

        /// <summary>
        /// Invoked when an item is removed from the inventory and should be placed on the ground.
        /// </summary>
        public Action<IInventoryItem> OnItemDropped;

        /// <summary>
        /// Invoked when the inventory is cleared.
        /// </summary>
        public Action OnCleared;

        /// <summary>
        /// Invoked when the inventory is resized.
        /// </summary>
        public Action OnResized;

        /// <summary>
        /// Returns the width of this inventory
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Returns the height of this inventory
        /// </summary>
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

        /// <summary>
        /// Returns true of given item is within this inventory
        /// </summary>
        /// <param name="item">Item to look for</param>
        public bool Contains(IInventoryItem item) { return _items.Contains(item); }

        /// <summary>
        /// Returns true if this inventory is full
        /// </summary>
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

        /// <summary>
        /// Returns true if its possible to add given item.
        /// </summary>
        /// <param name="item">Item to check</param>
        public bool CanAdd(IInventoryItem item)
        {
            if (_items.Contains(item)) return false;
            return GetFirstPointThatFitsItem(item).x != -1;
        }

        /// <summary>
        /// Add given item to the inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        public void Add(IInventoryItem item)
        {
            if (!CanAdd(item)) return;
            var freePoint = GetFirstPointThatFitsItem(item);
            if (freePoint.x == -1) return;
            AddAt(item, freePoint);
        }

        /// <summary>
        /// Returns true if its possible to add given item at given point within this inventory
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="point">Point at which to check</param>
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

        /// <summary>
        /// Add given item at point within inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="Point">Point at which to add item</param>
        public void AddAt(IInventoryItem item, Vector2Int Point)
        {
            if (!CanAdd(item)) return;
            if (CanAddAt(item, Point))
            {
                _items.Add(item);
                item.Shape.Position = Point;
                if (OnItemAdded != null) { OnItemAdded(item); }
            }
        }

        /// <summary>
        /// Returns true if its possible to remove given item
        /// </summary>
        /// <param name="item">Item to check</param>
        public bool CanRemove(IInventoryItem item)
        {
            return Contains(item);
        }

        /// <summary>
        /// Removes given item from this inventory
        /// </summary>
        /// <param name="item">Item to remove</param>
        public void Remove(IInventoryItem item)
        {
            if (CanRemove(item))
            {
                _items.Remove(item);
                if (OnItemRemoved != null) { OnItemRemoved(item); }
            }
        }

        /// <summary>
        /// Removes an item from this inventory and invokes OnItemDropped
        /// </summary>
        /// <param name="item"></param>
        public void Drop(IInventoryItem item)
        {
            Remove(item);
            if (OnItemDropped != null) { OnItemDropped(item); }
        }

        /// <summary>
        /// Drops all items in this inventory
        /// </summary>
        public void DropAll()
        {
            var itemsToDrop = _items.ToArray();
            foreach (var item in itemsToDrop)
            {
                Drop(item);
            }
        }

        /// <summary>
        /// Clears (destroys) all items in this inventory
        /// </summary>
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
                    if (CanAddAt(item, p)) return p;
                }
            }
            return new Vector2Int(-1, -1);
        }
    }
}