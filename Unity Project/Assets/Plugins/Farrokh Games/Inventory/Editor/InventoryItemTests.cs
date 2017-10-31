using NUnit.Framework;
using FarrokhGames.Shared;
using System.Linq;
using System;
using UnityEngine;

namespace FarrokhGames
{
    [TestFixture]
    public class InventoryItemTests
    {
        [Test]
        public void CTOR_NullShape_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new InventoryItem("name", null, null));
        }

        [Test]
        public void CTOR_Name_Set()
        {
            var item = new InventoryItem("MyName", null, new InventoryShape(1, 1));
            Assert.That(item.Name, Is.EqualTo("MyName"));
        }

        [Test]
        public void CTOR_Sprite_Set()
        {
            var sprite = new Sprite();
            var item = new InventoryItem("MyName", sprite, new InventoryShape(1, 1));
            Assert.That(item.Sprite, Is.SameAs(sprite));
        }

        [Test]
        public void CTOR_RectCalculated()
        {
            var item = new InventoryItem("MyName", null, new InventoryShape(2, 3));

            Assert.That(item.Rect.x, Is.EqualTo(0));
            Assert.That(item.Rect.y, Is.EqualTo(0));
            Assert.That(item.Rect.width, Is.EqualTo(2));
            Assert.That(item.Rect.height, Is.EqualTo(3));
        }

        [Test]
        public void CTOR_PointsCalculated()
        {
            var s = new bool[2, 2];
            s[0, 0] = true;
            s[1, 1] = true;

            var item = new InventoryItem("MyName", null, new InventoryShape(s));

            Assert.That(item.Points.Count, Is.EqualTo(2));
            Assert.That(item.Contains(Point.zero), Is.True);
            Assert.That(item.Contains(Point.one), Is.True);
        }

        [Test]
        public void Position_Changed()
        {
            var item = new InventoryItem("MyName", null, new InventoryShape(1, 1));
            item.Position = Point.one;
            Assert.That(item.Position, Is.EqualTo(Point.one));
        }

        [Test]
        public void Position_RectCalculated()
        {
            var item = new InventoryItem("MyName", null, new InventoryShape(3, 2));
            item.Position = new Point(6, 9);
            Assert.That(item.Rect.x, Is.EqualTo(6));
            Assert.That(item.Rect.y, Is.EqualTo(9));
            Assert.That(item.Rect.width, Is.EqualTo(3));
            Assert.That(item.Rect.height, Is.EqualTo(2));
        }

        [Test]
        public void Position_PointsCalculated()
        {
            var s = new bool[2, 2];
            s[0, 0] = true;
            s[1, 1] = true;

            var item = new InventoryItem("MyName", null, new InventoryShape(s));
            item.Position = new Point(6, 9);

            Assert.That(item.Points.Count, Is.EqualTo(2));
            Assert.That(item.Contains(new Point(6, 9)), Is.True);
            Assert.That(item.Contains(new Point(7, 10)), Is.True);
        }

        [Test]
        public void Overlaps_DontOverlap_ReturnsFalse()
        {
            var s1 = new bool[2, 2];
            s1[0, 0] = true;
            s1[0, 1] = true;
            s1[1, 1] = true;
            var item1 = new InventoryItem("MyName", null, new InventoryShape(s1));
            item1.Position = new Point(0, 0);

            var s2 = new bool[2, 2];
            s1[0, 1] = true;
            s1[1, 1] = true;
            s1[1, 0] = true;
            var item2 = new InventoryItem("MyName", null, new InventoryShape(s2));
            item2.Position = new Point(0, 1);

            Assert.That(item1.Overlaps(item2), Is.False);
            Assert.That(item2.Overlaps(item1), Is.False);
        }

        [Test]
        public void Overlaps_DoOverlap_ReturnsTrue()
        {
            var s1 = new bool[3, 1];
            s1[0, 0] = true;
            s1[1, 0] = true;
            s1[2, 0] = true;
            var item1 = new InventoryItem("MyName", null, new InventoryShape(s1));
            item1.Position = new Point(0, 0);

            var s2 = new bool[1, 3];
            s2[0, 0] = true;
            s2[0, 1] = true;
            s2[0, 2] = true;
            var item2 = new InventoryItem("MyName", null, new InventoryShape(s2));
            item2.Position = new Point(1, -1);

            Assert.That(item1.Overlaps(item2), Is.True);
            Assert.That(item2.Overlaps(item1), Is.True);
        }
    }
}