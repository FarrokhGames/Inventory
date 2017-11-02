using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Scriptable Object representing an Inventory Item
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class ItemDefinition : ScriptableObject, IInventoryItem
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private InventoryShape _shape;

        public string Name { get { return this.name; } }
        public Sprite Sprite { get { return _sprite; } }
        public InventoryShape Shape { get { return _shape; } }

        /// <summary>
        /// Creates a copy if this scriptable object
        /// </summary>
        public IInventoryItem CreateInstance()
        {
            return ScriptableObject.Instantiate(this);
        }
    }
}