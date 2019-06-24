using UnityEngine;

namespace FarrokhGames.Inventory
{
    public interface IInventoryItem
    {
        /// <summary>
        /// The sprite of this item
        /// </summary>
        Sprite Sprite { get; }

        /// <summary>
        /// Returns this items position within an inventory
        /// </summary>
        Vector2Int Position { get; set; }

        /// <summary>
        /// The width of this item
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of this item
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Returns true if given local position is part 
        /// of this items shape
        /// </summary>
        bool IsPartOfShape(Vector2Int localPosition);
    }

    internal static class IInventoryItemExtensions
    {
        /// <summary>
        /// Returns the lower left corner position of an item 
        /// within its inventory
        /// </summary>
        internal static Vector2Int GetMinPoint(this IInventoryItem item)
        {
            return item.Position;
        }

        /// <summary>
        /// Returns the top right corner position of an item 
        /// within its inventory
        /// </summary>
        internal static Vector2Int GetMaxPoint(this IInventoryItem item)
        {
            return item.Position + new Vector2Int(item.Width, item.Height);
        }

        /// <summary>
        /// Returns true if this item overlaps the given point within an inventory
        /// </summary>
        internal static bool Contains(this IInventoryItem item, Vector2Int inventoryPoint)
        {
            for (var iX = 0; iX < item.Width; iX++)
            {
                for (var iY = 0; iY < item.Height; iY++)
                {
                    var iPoint = item.Position + new Vector2Int(iX, iY);
                    if (iPoint == inventoryPoint) { return true; }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns true of this item overlaps a given item
        /// </summary>
        internal static bool Overlaps(this IInventoryItem item, IInventoryItem otherItem)
        {
            for (var iX = 0; iX < item.Width; iX++)
            {
                for (var iY = 0; iY < item.Height; iY++)
                {
                    if (item.IsPartOfShape(new Vector2Int(iX, iY)))
                    {
                        var iPoint = item.Position + new Vector2Int(iX, iY);
                        for (var oX = 0; oX < otherItem.Width; oX++)
                        {
                            for (var oY = 0; oY < otherItem.Height; oY++)
                            {
                                if (otherItem.IsPartOfShape(new Vector2Int(oX, oY)))
                                {
                                    var oPoint = otherItem.Position + new Vector2Int(oX, oY);
                                    if (oPoint == iPoint) { return true; } // Hit! Items overlap
                                }
                            }
                        }
                    }
                }
            }
            return false; // Items does not overlap
        }
    }
}