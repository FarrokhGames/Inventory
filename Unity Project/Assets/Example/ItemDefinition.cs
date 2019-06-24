using UnityEngine;

namespace FarrokhGames.Inventory.Examples
{
    /// <summary>
    /// Scriptable Object representing an Inventory Item
    /// </summary>
    [CreateAssetMenu(fileName = "Item", menuName = "Inventory/Item", order = 1)]
    public class ItemDefinition : ScriptableObject, IInventoryItem
    {
        [SerializeField] private Sprite _sprite = null;
        [SerializeField] private InventoryShape _shape = null;
        [SerializeField, HideInInspector] private Vector2Int _position = Vector2Int.zero;

        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get { return this.name; } }

        public InventoryShape Shape { get { return _shape; } } // TODO: REMOVE

        /// <inheritdoc />
        public Sprite Sprite { get { return _sprite; } }

        /// <inheritdoc />
        public int Width { get { return _shape.Width; } }

        /// <inheritdoc />
        public int Height { get { return _shape.Height; } }

        /// <inheritdoc />
        public Vector2Int Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <inheritdoc />
        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return _shape.IsPartOfShape(localPosition);
        }

        /// <summary>
        /// Creates a copy if this scriptable object
        /// </summary>
        public IInventoryItem CreateInstance()
        {
            return ScriptableObject.Instantiate(this);
        }
    }
}