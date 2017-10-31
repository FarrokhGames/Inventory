using System;
using System.Collections.Generic;
using FarrokhGames.Shared;
using UnityEngine;

namespace FarrokhGames
{
    /// <summary>
    /// Class for storing the shape of an inventory item
    /// </summary>
    [Serializable]
    public class InventoryShape
    {
        [SerializeField] int _width;
        [SerializeField] int _height;
        [SerializeField] bool[] _shape;
        private Point[] _points;
        private bool _haveBuiltPoints = false;

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
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="shape">A custom shape</param>
        public InventoryShape(bool[,] shape)
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
        }

        /// <summary>
        /// Returns the width of the shapes bounding box
        /// </summary>
        public int Width { get { return _width; } }

        /// <summary>
        /// Returns the height of the shapes bounding box
        /// </summary>
        /// <returns></returns>
        public int Height { get { return _height; } }

        /// <summary>
        /// Returns a list of points representing the shape in local space
        /// </summary>
        public Point[] Points
        {
            get
            {
                if (!_haveBuiltPoints)
                {
                    var p = new List<Point>();
                    for (int x = 0; x < Width; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (Get(x, y)) { p.Add(new Point(x, y)); }
                        }
                    }
                    _points = p.ToArray();
                    _haveBuiltPoints = true;
                }
                return _points;
            }
        }

        /*
        Returns if given internal point is free or occupied by the shape
        */
        private bool Get(int x, int y)
        {
            return _shape[GetIndex(x, y)];
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