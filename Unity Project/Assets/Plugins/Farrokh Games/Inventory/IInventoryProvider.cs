using System;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// A provider for a single inventory
    /// </summary>
    public interface IInventoryProvider
    {
        /// <summary>
        /// Invoked when something in the inventory changes 
        /// (items added, removed, etc.)
        /// </summary>
        Action OnInventoryChanged { get; set; }

        /// <summary>
        /// Returns the total amount of inventory items in 
        /// this inventory
        /// </summary>
        int InventoryItemCount { get; }

        /// <summary>
        /// Returns the inventory item at given index
        /// </summary>
        IInventoryItem GetInventoryItem(int index);

        /// <summary>
        /// Returns true if given inventory item is allowed inside 
        /// this inventory
        /// </summary>
        bool IsInventoryItemAllowed(IInventoryItem item);

        /// <summary>
        /// Invoked when an inventory item is added to the 
        /// inventory. Returns true if successful.
        /// </summary>
        bool AddInventoryItem(IInventoryItem item);

        /// <summary>
        /// Invoked when an inventory item is removed to the 
        /// inventory. Returns true if successful.
        /// </summary>
        bool RemoveInventoryItem(IInventoryItem item);

        /// <summary>
        /// Invoked when an inventory item is removed from the 
        /// inventory and should be placed on the ground.
        /// Returns true if successful.
        /// </summary>
        bool DropInventoryItem(IInventoryItem item);
    }
}