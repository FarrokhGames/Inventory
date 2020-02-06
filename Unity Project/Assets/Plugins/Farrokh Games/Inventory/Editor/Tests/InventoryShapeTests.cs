using NUnit.Framework;

namespace FarrokhGames.Inventory
{
    [TestFixture]
    public class InventoryShapeTests
    {
        [Test]
        public void CTOR_WidthAndHeightSet()
        {
            var shape = new InventoryShape(8, 12);

            Assert.That(shape.width, Is.EqualTo(8));
            Assert.That(shape.height, Is.EqualTo(12));
        }

        /*
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
        */

        /*
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
        */
    }
}