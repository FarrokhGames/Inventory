using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Example Lobby class
    /// </summary>
    [RequireComponent(typeof(InventoryRenderer))]
    public class SizeInventoryExample : MonoBehaviour
    {
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 4;
        [SerializeField] private ItemDefinition[] _definitions;
        [SerializeField] private bool _fillEmpty = false; // Should the inventory get completely filled?

        void Start()
        {
            // Create inventory
            var inventory = new InventoryManager(_width, _height);

            // Fill inventory with random items
            var tries = (_width * _height) / 3;
            for (var i = 0; i < tries; i++)
            {
                inventory.Add(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
            }

            // Fill empty slots with first (1x1) item
            if (_fillEmpty)
            {
                for (var i = 0; i < _width * _height; i++)
                {
                    inventory.Add(_definitions[0].CreateInstance());
                }
            }

            // Sets the renderers's inventory to trigger drawing
            GetComponent<InventoryRenderer>().SetInventory(inventory);

            // Log items being dropped on the ground
            inventory.OnItemDropped += (item) =>
            {
                Debug.Log(item.Name + " was dropped on the ground");
            };
        }
    }
}