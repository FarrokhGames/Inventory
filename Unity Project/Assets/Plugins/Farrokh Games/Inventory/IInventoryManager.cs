using System;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    public interface IInventoryManager
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
        /// Invoked when the inventory is cleared.
        /// </summary>
        Action OnCleared { get; set; }

        /// <summary>
        /// Invoked when the inventory is resized.
        /// </summary>
        Action OnResized { get; set; }

        /// <summary>
        /// Returns the width of this inventory
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Returns the height of this inventory
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Returns an array containing all items within 
        /// this inventory
        /// </summary>
        IInventoryItem[] GetAllItems();

        /// <summary>
        /// Returns true of given item is within this 
        /// inventory
        /// </summary>
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

        /// <summary>
        /// Returns true if its possible to add given item at 
        /// given point within this inventory
        /// </summary>
        bool CanAddAt(IInventoryItem item, Vector2Int point);

        /// <summary>
        /// Add given item at point within inventory. Returns 
        /// true if successful
        /// </summary>
        bool AddAt(IInventoryItem item, Vector2Int point);

        /// <summary>
        /// Returns true if its possible to remove given item
        /// </summary>
        bool CanRemove(IInventoryItem item);

        /// <summary>
        /// Removes given item from this inventory. Returns
        /// true if successful.
        /// </summary>
        bool Remove(IInventoryItem item);

        /// <summary>
        /// Returns true if given item can be dropped from 
        /// this inventory
        /// </summary>
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

        /// <summary>
        /// Get an item at given point within this inventory
        /// </summary>
        IInventoryItem GetAtPoint(Vector2Int point);
    }
}