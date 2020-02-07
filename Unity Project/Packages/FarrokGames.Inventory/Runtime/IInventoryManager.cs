using System;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    public interface IInventoryManager : IDisposable
    {
        /// <summary>
        /// Invoked when an item is added to the inventory
        /// </summary>
        Action<IInventoryItem> onItemAdded { get; set; }

        /// <summary>
        /// Invoked when an item is removed to the inventory
        /// </summary>
        Action<IInventoryItem> onItemRemoved { get; set; }

        /// <summary>
        /// Invoked when an item is removed from the inventory and should be placed on the ground.
        /// </summary>
        Action<IInventoryItem> onItemDropped { get; set; }

        /// <summary>
        /// Invoked when the inventory is rebuilt from scratch
        /// </summary>
        Action onRebuilt { get; set; }

        /// <summary>
        /// Invoked when the inventory changes its size
        /// </summary>
        Action onResized { get; set; }

        /// <summary>
        /// The width of the inventory
        /// </summary>
        int width { get; }

        /// <summary>
        /// The height of the inventory
        /// </summary>
        int height { get; }

        /// <summary>
        /// Sets a new width and height of the inventory
        /// </summary>
        void Resize(int width, int height);

        /// <summary>
        /// Returns all items inside this inventory
        /// </summary>
        IInventoryItem[] allItems { get; }

        /// <summary>
        /// Returns true if given item is present in this inventory
        /// </summary>
        bool Contains(IInventoryItem item);

        /// <summary>
        /// Returns true if this inventory is full
        /// </summary>
        bool isFull { get; }

        /// <summary>
        /// Returns true if its possible to add given item
        /// </summary>
        bool CanAdd(IInventoryItem item);

        /// <summary>
        /// Add given item to the inventory. Returns true
        /// if successful
        /// </summary>
        bool TryAdd(IInventoryItem item);

        /// <summary>
        /// Returns true if its possible to add item at location
        /// </summary>
        bool CanAddAt(IInventoryItem item, Vector2Int point);

        /// <summary>
        /// Tries to add item att location and returns true if successful
        /// </summary>
        bool TryAddAt(IInventoryItem item, Vector2Int point);

        /// <summary>
        /// Returns true if its possible to remove this item
        /// </summary>
        bool CanRemove(IInventoryItem item);

        /// <summary>
        /// Returns true ifits possible to swap this item
        /// </summary>
        bool CanSwap(IInventoryItem item);

        /// <summary>
        /// Removes given item from this inventory. Returns
        /// true if successful.
        /// </summary>
        bool TryRemove(IInventoryItem item);

        /// <summary>
        /// Returns true if its possible to drop this item
        /// </summary>
        bool CanDrop(IInventoryItem item);

        /// <summary>
        /// Removes an item from this inventory. Returns true
        /// if successful.
        /// </summary>
        bool TryDrop(IInventoryItem item);

        /// <summary>
        /// Drops all items from this inventory
        /// </summary>
        void DropAll();

        /// <summary>
        /// Clears (destroys) all items in this inventory
        /// </summary>
        void Clear();

        /// <summary>
        /// Rebuilds the inventory
        /// </summary>
        void Rebuild();

        /// <summary>
        /// Get an item at given point within this inventory
        /// </summary>
        IInventoryItem GetAtPoint(Vector2Int point);

        /// <summary>
        /// Returns all items under given rectangle
        /// </summary>
        IInventoryItem[] GetAtPoint(Vector2Int point, Vector2Int size);
    }
}