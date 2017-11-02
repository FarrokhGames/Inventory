using UnityEngine;

namespace FarrokhGames.Inventory
{
    public class TestItem : IInventoryItem
    {
        public string Name { get; private set; }
        public Sprite Sprite { get; private set; }
        public InventoryShape Shape { get; private set; }

        public TestItem(string name, Sprite sprite, InventoryShape shape)
        {
            Name = name;
            Sprite = sprite;
            Shape = shape;
        }
    }
}