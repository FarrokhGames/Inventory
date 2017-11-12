using System;
using System.Collections.Generic;
using System.Linq;
using FarrokhGames.Shared;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Class for storing the shape and position of an inventory item
    /// </summary>
    [Serializable]
    public class InventoryShape
    {
        [SerializeField] int _width;
        [SerializeField] int _height;
        [SerializeField] bool[] _shape;

        private Vector2Int _position = Vector2Int.zero;
        private Vector2Int[] _originalPoints;
        private bool _haveBuiltOriginalPoints = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">The maximum width of the shape</param>
        /// <param name="height">The maximum height of the shape</param>
        public InventoryShape(int width, int height)
        {
            _width = width;
            _height = height;
            _shape = new bool[_width * _height];
            Recalculate();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="shape">A custom shape</param>
        public InventoryShape(bool[, ] shape)
        {
            _width = shape.GetLength(0);
            _height = shape.GetLength(1);
            _shape = new bool[_width * _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _shape[GetIndex(x, y)] = shape[x, y];
                }
            }
            Recalculate();
        }

        /// <summary>
        /// Returns the width of the shapes bounding box
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// Returns the height of the shapes bounding box
        /// </summary>
        public int Height { get { return _height; } }

        /// <summary>
        /// Gets or sets the positon of this shape
        /// </summary>
        public Vector2Int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Recalculate();
            }
        }

        /// <summary>
        /// Returns the rect for this shape (adjusted by its current position)
        /// </summary>
        public Rect Rect { get; private set; }

        /// <summary>
        /// Returns the points making up this shape, adjusted for current position
        /// </summary>
        public Vector2Int[] Points { get; private set; }

        /*
        Recalculates the rectangle and points and adjusts them by the current position)
        */
        private void Recalculate()
        {
            // Create adjusted rect
            Rect = new Rect(Position, Vector2.Max(Vector2.one, new Vector2(Width, Height)));

            // Build original points (This only happens once)
            if (!_haveBuiltOriginalPoints)
            {
                var p = new List<Vector2Int>();
                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (_shape[GetIndex(x, y)])
                        {
                            p.Add(new Vector2Int(x, y));
                        }
                    }
                }
                _originalPoints = p.ToArray();
                _haveBuiltOriginalPoints = true;
            }

            // Create adjusted points
            Points = new Vector2Int[_originalPoints.Length];
            for (int i = 0; i < _originalPoints.Length; i++)
            {
                Points[i] = _originalPoints[i] + Position;
            }
        }

        /// <summary>
        /// Returns true of this item is occupying the given point
        /// </summary>
        /// <param name="point">The point to check</param>
        public bool Contains(Vector2Int point)
        {
            return Points.Contains(point);
        }

        /// <summary>
        /// Returns true of this item overlaps the given item
        /// </summary>
        /// <param name="otherShape">Other shape to check</param>
        public bool Overlaps(InventoryShape otherShape)
        {
            if (Rect.Overlaps(otherShape.Rect)) // Check rect first since its faster
            {
                // Check point by point to account for shape
                for (int i = 0; i < Points.Length; i++)
                {
                    if (otherShape.Contains(Points[i]))
                    {
                        return true; // Items overlap
                    }
                }
            }
            return false; // Items does not overlap
        }

        /*
        Converts X & Y to an index to use with _shape
        */
        private int GetIndex(int x, int y)
        {
            y = (_height - 1) - y;
            return x + _width * y;
        }
    }
}