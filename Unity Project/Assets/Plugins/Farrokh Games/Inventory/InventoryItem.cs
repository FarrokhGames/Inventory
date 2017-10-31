using System;
using System.Linq;
using FarrokhGames.Shared;
using UnityEngine;

namespace FarrokhGames
{
    /// <summary>
    /// Class representing an item in the inventory
    /// </summary>
    public class InventoryItem
    {
        /// <summary>
        /// Returns the name of the item
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns the items sprite
        /// </summary>
        public Sprite Sprite { get; private set; }

        /// <summary>
        /// Returns the rect for this item (adjusted by the items current position)
        /// </summary>
        public Rect Rect { get; private set; }

        /// <summary>
        /// Returns the points representing the items shape (adjusted by the items current position)
        /// </summary>
        public Point[] Points { get; private set; }

        private Point _position = Point.zero;
        private InventoryShape _shape;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">The name of the item</param>
        /// <param name="sprite">The sprite used to render the item in the inventory</param>
        /// <param name="shape">The shape of the item</param>
        public InventoryItem(string name, Sprite sprite, InventoryShape shape)
        {
            if (shape == null) { throw new ArgumentNullException("shape"); }

            Name = name;
            Sprite = sprite;
            _shape = shape;
            RecalculateRect();
            RecalculatePoints();
        }

        /*
        Recalculates the rectangle and adjusts it by the items current position)
        */
        private void RecalculateRect()
        {
            Rect = new Rect(Position, Vector2.Max(Vector2.one, new Vector2(_shape.Width, _shape.Height)));
        }

        /*
        Recalculates the points represening the shape of the item, and adjusts it by the current position)
        */
        private void RecalculatePoints()
        {
            Points = new Point[_shape.Points.Length];
            for (int i = 0; i < _shape.Points.Length; i++)
            {
                Points[i] = _shape.Points[i] + Position;
            }
        }

        /// <summary>
        /// Returns the width of the items bounding box
        /// </summary>
        public int Width { get { return _shape.Width; } }

        /// <summary>
        /// Returns the height of the items bounding box
        /// </summary>
        public int Height { get { return _shape.Height; } }

        /// <summary>
        /// Gets or sets the positon of this item
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RecalculateRect();
                RecalculatePoints();
            }
        }

        /// <summary>
        /// Returns true of this item is occupying the given point
        /// </summary>
        /// <param name="point">The point to check</param>
        public bool Contains(Point point)
        {
            return Points.Contains(point);
        }

        /// <summary>
        /// Returns true of this item overlaps the given item
        /// </summary>
        /// <param name="otherItem">Item to check</param>
        public bool Overlaps(InventoryItem otherItem)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Rect.Overlaps(otherItem.Rect)) // Check rect first since its faster
                {
                    // Check point by point to account for shape
                    for (int j = 0; j < otherItem.Points.Length; j++)
                    {
                        if (Points[i] == otherItem.Points[j]) { return true; } // Item overlaps
                    }
                }
            }
            return false; // Item does not overlap
        }
    }
}