using UnityEngine;

namespace FarrokhGames.Inventory
{
    public interface IInventoryItem
    {
        string Name { get; }
        Sprite Sprite { get; }
        InventoryShape Shape { get; }
    }
}