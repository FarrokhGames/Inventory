using UnityEngine;

namespace FarrokhGames.Inventory
{
    public class TestItem : IInventoryItem
    {
        public string Name { get; private set; } // TODO: REMOVE
        public InventoryShape Shape { get; private set; } // TODO: REMOVE

        public Sprite Sprite { get; private set; }

        public int Width { get { return Shape.Width; } }

        public int Height { get { return Shape.Height; } }

        public Vector2Int Position { get; set; }

        public TestItem(string name, Sprite sprite, InventoryShape shape)
        {
            Name = name;
            Sprite = sprite;
            Shape = shape;
            Position = Vector2Int.zero;
        }

        public bool IsPartOfShape(Vector2Int localPosition)
        {
            return Shape.IsPartOfShape(localPosition);
        }
    }
}