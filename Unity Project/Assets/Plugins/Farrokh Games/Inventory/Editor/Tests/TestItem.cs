using UnityEngine;

namespace FarrokhGames.Inventory
{
    public class TestItem : IInventoryItem
    {
        private InventoryShape _shape;

        public TestItem(string name, Sprite sprite, InventoryShape shape)
        {
            Sprite = sprite;
            _shape = shape;
            Position = Vector2Int.zero;
        }

        public Sprite Sprite { get; private set; }
        public int Width { get { return _shape.Width; } }
        public int Height { get { return _shape.Height; } }
        public Vector2Int Position { get; set; }

        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return _shape.IsPartOfShape(localPosition);
        }
    }
}