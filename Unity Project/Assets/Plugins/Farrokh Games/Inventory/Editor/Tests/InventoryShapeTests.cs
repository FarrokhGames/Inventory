using System.Linq;
using FarrokhGames.Shared;
using NUnit.Framework;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    [TestFixture]
    public class InventoryShapeTests
    {
        [Test]
        public void CTOR_WidthAndHeightSet()
        {
            var shape = new InventoryShape(8, 12);

            Assert.That(shape.Width, Is.EqualTo(8));
            Assert.That(shape.Height, Is.EqualTo(12));
        }

        [Test]
        public void CTOR_Shape_WidthHeightAndPointsSet()
        {
            var s = new bool[3, 2];
            s[0, 0] = true;
            s[1, 1] = true;
            s[2, 0] = true;

            var shape = new InventoryShape(s);

            Assert.That(shape.Width, Is.EqualTo(3));
            Assert.That(shape.Height, Is.EqualTo(2));
            Assert.That(shape.Points.Length, Is.EqualTo(3));
        }

        [Test]
        public void Points_ReturnsCorrectPoints()
        {
            var s = new bool[3, 2];
            s[0, 0] = true;
            s[1, 1] = true;
            s[2, 0] = true;

            var shape = new InventoryShape(s);

            Assert.That(shape.Points.Length, Is.EqualTo(3));
            Assert.That(shape.Points.Contains(new Vector2Int(0, 0)), Is.True);
            Assert.That(shape.Points.Contains(new Vector2Int(1, 1)), Is.True);
            Assert.That(shape.Points.Contains(new Vector2Int(2, 0)), Is.True);
        }

        [Test]
        public void CTOR_RectCalculated()
        {
            var shape = new InventoryShape(2, 3);

            Assert.That(shape.Rect.x, Is.EqualTo(0));
            Assert.That(shape.Rect.y, Is.EqualTo(0));
            Assert.That(shape.Rect.width, Is.EqualTo(2));
            Assert.That(shape.Rect.height, Is.EqualTo(3));
        }

        [Test]
        public void CTOR_PointsCalculated()
        {
            var s = new bool[2, 2];
            s[0, 0] = true;
            s[1, 1] = true;

            var shape = new InventoryShape(s);

            Assert.That(shape.Points.Count, Is.EqualTo(2));
            Assert.That(shape.Contains(Vector2Int.zero), Is.True);
            Assert.That(shape.Contains(Vector2Int.one), Is.True);
        }

        [Test]
        public void Position_Changed()
        {
            var shape = new InventoryShape(1, 1);
            shape.Position = Vector2Int.one;
            Assert.That(shape.Position, Is.EqualTo(Vector2Int.one));
        }

        [Test]
        public void Position_RectCalculated()
        {
            var shape = new InventoryShape(3, 2);
            shape.Position = new Vector2Int(6, 9);
            Assert.That(shape.Rect.x, Is.EqualTo(6));
            Assert.That(shape.Rect.y, Is.EqualTo(9));
            Assert.That(shape.Rect.width, Is.EqualTo(3));
            Assert.That(shape.Rect.height, Is.EqualTo(2));
        }

        [Test]
        public void Position_PointsCalculated()
        {
            var s = new bool[2, 2];
            s[0, 0] = true;
            s[1, 1] = true;

            var shape = new InventoryShape(s);
            shape.Position = new Vector2Int(6, 9);

            Assert.That(shape.Points.Count, Is.EqualTo(2));
            Assert.That(shape.Contains(new Vector2Int(6, 9)), Is.True);
            Assert.That(shape.Contains(new Vector2Int(7, 10)), Is.True);
        }

        [Test]
        public void Overlaps_DontOverlap_ReturnsFalse()
        {
            var s1 = new bool[2, 2];
            s1[0, 0] = true;
            s1[0, 1] = true;
            s1[1, 1] = true;
            var shape1 = new InventoryShape(s1);
            shape1.Position = new Vector2Int(0, 0);

            var s2 = new bool[2, 2];
            s1[0, 1] = true;
            s1[1, 1] = true;
            s1[1, 0] = true;
            var shape2 = new InventoryShape(s2);
            shape2.Position = new Vector2Int(0, 1);

            Assert.That(shape1.Overlaps(shape2), Is.False);
            Assert.That(shape2.Overlaps(shape1), Is.False);
        }

        [Test]
        public void Overlaps_DoOverlap_ReturnsTrue()
        {
            var s1 = new bool[3, 1];
            s1[0, 0] = true;
            s1[1, 0] = true;
            s1[2, 0] = true;
            var shape1 = new InventoryShape(s1);
            shape1.Position = new Vector2Int(0, 0);

            var s2 = new bool[1, 3];
            s2[0, 0] = true;
            s2[0, 1] = true;
            s2[0, 2] = true;
            var shape2 = new InventoryShape(s2);
            shape2.Position = new Vector2Int(1, -1);

            Assert.That(shape1.Overlaps(shape2), Is.True);
            Assert.That(shape2.Overlaps(shape1), Is.True);
        }
    }
}