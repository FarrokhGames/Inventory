using UnityEngine;

namespace FarrokhGames.Shared
{
    /// <summary>
    /// Representation of 2D vectors and points using integers
    /// </summary>
    [System.Serializable]
    public struct Point
    {
        /// <summary>
        /// Shorthand for writing Point(1, 1).
        /// </summary>
        public static readonly Point one = new Point(1, 1);

        /// <summary>
        /// Shorthand for writing Point(0, 0).
        /// </summary>
        public static readonly Point zero = new Point(0, 0);

        /// <summary>
        /// Shorthand for writing Point(-1, 0).
        /// </summary>
        public static readonly Point left = new Point(-1, 0);

        /// <summary>
        /// Shorthand for writing Point(1, 0).
        /// </summary>
        public static readonly Point right = new Point(1, 0);

        /// <summary>
        /// Shorthand for writing Point(0, 1).
        /// </summary>
        public static readonly Point up = new Point(0, 1);

        /// <summary>
        /// Shorthand for writing Point(0, -1).
        /// </summary>
        public static readonly Point down = new Point(0, -1);

        /// <summary>
        /// X component of the vector.
        /// </summary>
        public int x;

        /// <summary>
        /// Y component of the vector.
        /// </summary>
        public int y;

        /// <summary>
        /// Constructs a new point with given x, y components
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Returns true if the given point is exactly equal to this point.
        /// </summary>
        /// <param name="obj">Other object</param>
        public override bool Equals(object obj)
        {
            return (obj is Point) && (this == (Point)obj);
        }

        /// <summary>
        /// Returns the hashcode of this point
        /// </summary>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        /// <summary>
        /// Returns a nicely formatted string for this point.
        /// </summary>
        public override string ToString()
        {
            return "Point(" + x + "," + y + ")";
        }

        /// <summary>
        /// Converts this point into a Vector2
        /// </summary>
        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        /// <summary>
        /// Converts this point into a Vector3
        /// </summary>
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, 0);
        }

        /// <summary>
        /// Returns the distance between two points
        /// </summary>
        /// <param name="a">Point A</param>
        /// <param name="b">Point B</param>
        public static float Distance(Point a, Point b)
        {
            return Mathf.Sqrt((Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2)));
        }

        /*
		Equals Operator
		*/
        public static bool operator ==(Point a, Point b)
        {
            return a.x == b.x && a.y == b.y;
        }

        /*
		Not Equals Operator
		*/
        public static bool operator !=(Point a, Point b)
        {
            return !(a == b);
        }

        /*
		Add Operator
		*/
        public static Point operator +(Point a, Point b)
        {
            return new Point(a.x + b.x, a.y + b.y);
        }

        /*
		Add Operator (Vector2)
		*/
        public static Point operator +(Point a, Vector2 b)
        {
            return new Point(a.x + (int)b.x, a.y + (int)b.y);
        }

        /*
		Add Operator (Vector3)
		*/
        public static Point operator +(Point a, Vector3 b)
        {
            return new Point(a.x + (int)b.x, a.y + (int)b.y);
        }

        /*
		Subtract Operator
		*/
        public static Point operator -(Point a, Point b)
        {
            return new Point(a.x - b.x, a.y - b.y);
        }

        /*
		Subtract Operator
		*/
        public static Point operator -(Point a)
        {
            return new Point(-a.x, -a.y);
        }

        /*
		Subtract Operator (Between Point and Vector2)
		*/
        public static Point operator -(Point a, Vector2 b)
        {
            return new Point(a.x - (int)b.x, a.y - (int)b.y);
        }

        /*
		Subtract Operator (Between Point and Vector3)
		*/
        public static Point operator -(Point a, Vector3 b)
        {
            return new Point(a.x - (int)b.x, a.y - (int)b.y);
        }

        /*
		Multiplication Operator (Between Point and int)
		*/
        public static Point operator *(Point a, int b)
        {
            return new Point(a.x * b, a.y * b);
        }

        /*
		Multiplication Operator (Between two points)
		*/
        public static Point operator *(Point a, Point b)
        {
            return new Point(a.x * b.x, a.y * b.y);
        }

        /*
		Multiplication Operator (Between Point and Vector2)
		*/
        public static Point operator *(Point a, Vector2 b)
        {
            return new Point(a.x * (int)b.x, a.y * (int)b.y);
        }

        /*
		Multiplication Operator (Between Point and Vector3)
		*/
        public static Point operator *(Point a, Vector3 b)
        {
            return new Point(a.x * (int)b.x, a.y * (int)b.y);
        }

        /*
		Division Operator
		*/
        public static Point operator /(Point a, int d)
        {
            return new Point(a.x / d, a.y / d);
        }

        /*
		Division Operator (With Point)
		*/
        public static Point operator /(Point a, Point b)
        {
            return new Point(a.x / b.x, a.y / b.y);
        }

        /*
		Division Operator (With Vector2)
		*/
        public static Point operator /(Point a, Vector2 b)
        {
            return new Point(a.x / (int)b.x, a.y / (int)b.y);
        }

        /*
		Division Operator (With Vector3)
		*/
        public static Point operator /(Point a, Vector3 b)
        {
            return new Point(a.x / (int)b.x, a.y / (int)b.y);
        }

        /*
		Implicit Operator (From Vector2 to Point)
		*/
        public static implicit operator Point(Vector2 v)
        {
            return new Point((int)v.x, (int)v.y);
        }

        /*
		Implicit Operator (From Point to Vector2)
		*/
        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.x, p.y);
        }

        /*
		Implicit Operator (From Vector3 to Point)
		*/
        public static implicit operator Point(Vector3 v)
        {
            return new Point((int)v.x, (int)v.y);
        }

        /*
		Implicit Operator (From Point to Vector3)
		*/
        public static implicit operator Vector3(Point p)
        {
            return new Vector3(p.x, p.y, 0);
        }
    }
}