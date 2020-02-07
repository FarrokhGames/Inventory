namespace FarrokhGames.Inventory
{
    /// <summary>
    /// A provider for a single inventory
    /// </summary>
    public interface IInventoryProvider
    {
        /// <summary>
        /// Returns the render mode of this inventory
        /// </summary>
        InventoryRenderMode inventoryRenderMode { get; }

        /// <summary>
        /// Returns the total amount of inventory items in 
        /// this inventory
        /// </summary>
        int inventoryItemCount { get; }

        /// <summary>
        /// Returns true if the inventory is full
        /// </summary>
        bool isInventoryFull { get; }

        /// <summary>
        /// Returns the inventory item at given index
        /// </summary>
        IInventoryItem GetInventoryItem(int index);

        /// <summary>
        /// Returns true if given inventory item is allowed inside 
        /// this inventory
        /// </summary>
        bool CanAddInventoryItem(IInventoryItem item);

        /// <summary>
        /// Returns true if given inventory item is allowed to 
        /// be removed from this inventory
        /// </summary>
        bool CanRemoveInventoryItem(IInventoryItem item);

        /// <summary>
        /// Returns true if given inventory item is allowed to 
        /// be dropped on the ground
        /// </summary>
        bool CanDropInventoryItem(IInventoryItem item);

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