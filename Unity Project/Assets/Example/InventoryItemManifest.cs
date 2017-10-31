using UnityEngine;

namespace FarrokhGames.Examples
{
    /// <summary>
    /// Scriptable Object representing an Inventory Item
    /// </summary>
    public class InventoryItemManifest : ScriptableObject
    {
        [SerializeField] private Sprite _sprite;
        [SerializeField] private InventoryShape _shape;

        /// <summary>
        /// Converts this scriptable object into an InventoryItem
        /// </summary>
        public InventoryItem GetItem()
        {
            return new InventoryItem(this.name, _sprite, _shape);
        }
    }
}