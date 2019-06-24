using System;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    public interface IInventoryManager : IDisposable
    {
        /// <summary>
        /// Invoked when an item is added to the inventory
        /// </summary>
        Action<IInventoryItem> OnItemAdded { get; set; }

        /// <summary>
        /// Invoked when an item is removed to the inventory
        /// </summary>
        Action<IInventoryItem> OnItemRemoved { get; set; }

        /// <summary>
        /// Invoked when an item is removed from the inventory and should be placed on the ground.
        /// </summary>
        Action<IInventoryItem> OnItemDropped { get; set; }

        /// <summary>
        /// Invoked when the inventory is rebuilt from scratch
        /// </summary>
        Action OnRebuilt { get; set; }

        Action OnResized { get; set; }

        int Width { get; }

        int Height { get; }

        void Resize(int width, int height);

        IInventoryItem[] AllItems { get; }

        bool Contains(IInventoryItem item);

        /// <summary>
        /// Returns true if this inventory is full
        /// </summary>
        bool IsFull { get; }

        /// <summary>
        /// Returns true if its possible to add given item
        /// </summary>
        bool CanAdd(IInventoryItem item);

        /// <summary>
        /// Add given item to the inventory. Returns true
        /// if successful
        /// </summary>
        bool Add(IInventoryItem item);

        bool CanAddAt(IInventoryItem item, Vector2Int point);

        bool AddAt(IInventoryItem item, Vector2Int Point);

        bool CanRemove(IInventoryItem item);

        /// <summary>
        /// Removes given item from this inventory. Returns
        /// true if successful.
        /// </summary>
        bool Remove(IInventoryItem item);

        bool CanDrop(IInventoryItem item);

        /// <summary>
        /// Removes an item from this inventory. Returns true
        /// if successful.
        /// </summary>
        bool Drop(IInventoryItem item);

        /// <summary>
        /// Drops all items from this inventory
        /// </summary>
        void DropAll();

        /// <summary>
        /// Clears (destroys) all items in this inventory
        /// </summary>
        void Clear();

        void Rebuild();

        /// <summary>
        /// Get an item at given point within this inventory
        /// </summary>
        IInventoryItem GetAtPoint(Vector2Int point);
    }
}