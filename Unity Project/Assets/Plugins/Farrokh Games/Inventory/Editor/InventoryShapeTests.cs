using NUnit.Framework;
using FarrokhGames.Shared;
using System.Linq;

namespace FarrokhGames
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
            Assert.That(shape.Points.Contains(new Point(0, 0)), Is.True);
            Assert.That(shape.Points.Contains(new Point(1, 1)), Is.True);
            Assert.That(shape.Points.Contains(new Point(2, 0)), Is.True);
        }
    }
}