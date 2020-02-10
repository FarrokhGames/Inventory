using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Example Lobby class
    /// </summary>
    [RequireComponent(typeof(InventoryRenderer))]
    public class SizeInventoryExample : MonoBehaviour
    {
        [SerializeField] private InventoryRenderMode _renderMode = InventoryRenderMode.Grid;
        [SerializeField] private int _maximumAlowedItemCount = -1;
        [SerializeField] private ItemType _allowedItem = ItemType.Any;
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 4;
        [SerializeField] private ItemDefinition[] _definitions = null;
        [SerializeField] private bool _fillRandomly = true; // Should the inventory get filled with random items?
        [SerializeField] private bool _fillEmpty = false; // Should the inventory get completely filled?

        void Start()
        {
            var provider = new InventoryProvider(_renderMode, _maximumAlowedItemCount, _allowedItem);

            // Create inventory
            var inventory = new InventoryManager(provider, _width, _height);

            // Fill inventory with random items
            if (_fillRandomly)
            {
                var tries = (_width * _height) / 3;
                for (var i = 0; i < tries; i++)
                {
                    inventory.TryAdd(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
                }
            }

            // Fill empty slots with first (1x1) item
            if (_fillEmpty)
            {
                for (var i = 0; i < _width * _height; i++)
                {
                    inventory.TryAdd(_definitions[0].CreateInstance());
                }
            }

            // Sets the renderers's inventory to trigger drawing
            GetComponent<InventoryRenderer>().SetInventory(inventory, provider.inventoryRenderMode);

            // Log items being dropped on the ground
            inventory.onItemDropped += (item) =>
            {
                Debug.Log((item as ItemDefinition).Name + " was dropped on the ground");
            };

			// Log when an item was unable to be placed on the ground (due to its canDrop being set to false)
            inventory.onItemDroppedFailed += (item) =>
            {
                Debug.Log($"You're not allowed to drop {(item as ItemDefinition).Name} on the ground");
            };

			// Log when an item was unable to be placed on the ground (due to its canDrop being set to false)
            inventory.onItemAddedFailed += (item) =>
            {
                Debug.Log($"You can't put {(item as ItemDefinition).Name} there!");
            };
        }
    }
}