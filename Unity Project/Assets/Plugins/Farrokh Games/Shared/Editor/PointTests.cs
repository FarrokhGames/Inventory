using NUnit.Framework;
using UnityEngine;

namespace FarrokhGames.Shared
{
    [TestFixture]
    public class PointTests
    {
        [Test]
        public void One_ReturnsCorrectValue()
        {
            Assert.That(Point.one.x, Is.EqualTo(1));
            Assert.That(Point.one.y, Is.EqualTo(1));
        }

        [Test]
        public void Zero_ReturnsCorrectValue()
        {
            Assert.That(Point.zero.x, Is.EqualTo(0));
            Assert.That(Point.zero.y, Is.EqualTo(0));
        }

        [Test]
        public void Left_ReturnsCorrectValue()
        {
            Assert.That(Point.left.x, Is.EqualTo(-1));
            Assert.That(Point.left.y, Is.EqualTo(0));
        }

        [Test]
        public void Right_ReturnsCorrectValue()
        {
            Assert.That(Point.right.x, Is.EqualTo(1));
            Assert.That(Point.right.y, Is.EqualTo(0));
        }

        [Test]
        public void Up_ReturnsCorrectValue()
        {
            Assert.That(Point.up.x, Is.EqualTo(0));
            Assert.That(Point.up.y, Is.EqualTo(1));
        }

        [Test]
        public void Down_ReturnsCorrectValue()
        {
            Assert.That(Point.down.x, Is.EqualTo(0));
            Assert.That(Point.down.y, Is.EqualTo(-1));
        }

        [TestCase(0, 0)]
        [TestCase(2, 5)]
        [TestCase(-5, 2)]
        [TestCase(9, -8)]
        [TestCase(-3, -4)]
        public void CTOR_XYComponentsSet(int x, int y)
        {
            var point = new Point(x, y);
            Assert.That(point.x, Is.EqualTo(x));
            Assert.That(point.y, Is.EqualTo(y));
        }

        [Test]
        public void Equals_NoPoint_ReturnsFalse()
        {
            Assert.That(Point.zero.Equals(false), Is.False);
        }

        [Test]
        public void Equals_NotSame_ReturnsFalse()
        {
            Assert.That(Point.zero.Equals(Point.one), Is.False);
        }

        [Test]
        public void Equals_IsSame_ReturnsTrue()
        {
            Assert.That(Point.zero.Equals(Point.zero), Is.True);
            Assert.That(Point.one.Equals(Point.one), Is.True);
            Assert.That(Point.left.Equals(Point.left), Is.True);
            Assert.That(Point.up.Equals(Point.up), Is.True);
        }

        [Test]
        public void GetHashCode_ReturnsCurrectValue()
        {
            Assert.That(Point.one.GetHashCode(), Is.EqualTo(0));
            Assert.That(Point.zero.GetHashCode(), Is.EqualTo(0));
            Assert.That(new Point(32, -12).GetHashCode(), Is.EqualTo(-44));
        }

        [Test]
        public void ToString_ReturnsCorrectlyFormattedString()
        {
            Assert.That(Point.zero.ToString(), Is.EqualTo("Point(0,0)"));
            Assert.That(Point.one.ToString(), Is.EqualTo("Point(1,1)"));
            Assert.That(Point.left.ToString(), Is.EqualTo("Point(-1,0)"));
        }

        [Test]
        public void ToVector2_ReturnsCorrect()
        {
            Assert.That(Point.zero.ToVector2(), Is.EqualTo(new Vector2(0, 0)));
            Assert.That(Point.one.ToVector2(), Is.EqualTo(new Vector2(1, 1)));
            Assert.That(Point.left.ToVector2(), Is.EqualTo(new Vector2(-1, 0)));
            Assert.That(Point.up.ToVector2(), Is.EqualTo(new Vector2(0, 1)));
        }

        [Test]
        public void ToVector3_ReturnsCorrect()
        {
            Assert.That(Point.zero.ToVector3(), Is.EqualTo(new Vector3(0, 0, 0)));
            Assert.That(Point.one.ToVector3(), Is.EqualTo(new Vector3(1, 1, 0)));
            Assert.That(Point.left.ToVector3(), Is.EqualTo(new Vector3(-1, 0, 0)));
            Assert.That(Point.up.ToVector3(), Is.EqualTo(new Vector3(0, 1, 0)));
        }

        [Test]
        public void Distance_ReturnsCorrectValue()
        {
            Assert.That(Point.Distance(Point.zero, Point.one), Is.EqualTo(1.41421354f));
            Assert.That(Point.Distance(Point.zero, Point.left), Is.EqualTo(1));
            Assert.That(Point.Distance(Point.zero, Point.up), Is.EqualTo(1));
            Assert.That(Point.Distance(Point.zero, Point.zero), Is.EqualTo(0));
            Assert.That(Point.Distance(Point.one, Point.one), Is.EqualTo(0));
            Assert.That(Point.Distance(new Point(8, -4), new Point(-7, 11)), Is.EqualTo(21.2132034f));
        }

        [Test]
        public void Operator_Equals()
        {
            Assert.That(Point.zero == new Point(0, 0), Is.True);
            Assert.That(Point.one == Point.zero, Is.False);
        }

        [Test]
        public void Operator_NotEquals()
        {
            Assert.That(Point.zero != new Point(0, 0), Is.False);
            Assert.That(Point.one != Point.zero, Is.True);
        }

        [Test]
        public void Operator_Add()
        {
            Assert.That(Point.zero + Point.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero + Point.one, Is.EqualTo(Point.one));
            Assert.That(Point.one + Point.one, Is.EqualTo(new Point(2, 2)));
            Assert.That(Point.left + Point.left, Is.EqualTo(new Point(-2, 0)));
            Assert.That(Point.up + Point.up, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Add_Vector2()
        {
            Assert.That(Point.zero + Vector2.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero + Vector2.one, Is.EqualTo(Point.one));
            Assert.That(Point.one + Vector2.one, Is.EqualTo(new Point(2, 2)));
            Assert.That(Point.left + Vector2.left, Is.EqualTo(new Point(-2, 0)));
            Assert.That(Point.up + Vector2.up, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Add_Vector3()
        {
            Assert.That(Point.zero + Vector3.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero + Vector3.one, Is.EqualTo(Point.one));
            Assert.That(Point.one + Vector3.one, Is.EqualTo(new Point(2, 2)));
            Assert.That(Point.left + Vector3.left, Is.EqualTo(new Point(-2, 0)));
            Assert.That(Point.up + Vector3.up, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Subtract()
        {
            Assert.That(Point.zero - Point.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero - Point.one, Is.EqualTo(-Point.one));
            Assert.That(Point.one - Point.one, Is.EqualTo(Point.zero));
            Assert.That(Point.right - Point.left, Is.EqualTo(new Point(2, 0)));
            Assert.That(Point.up - Point.down, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Subtract_Vector2()
        {
            Assert.That(Point.zero - Vector2.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero - Vector2.one, Is.EqualTo(-Point.one));
            Assert.That(Point.one - Vector2.one, Is.EqualTo(Point.zero));
            Assert.That(Point.right - Vector2.left, Is.EqualTo(new Point(2, 0)));
            Assert.That(Point.up - Vector2.down, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Subtract_Vector3()
        {
            Assert.That(Point.zero - Vector3.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.zero - Vector3.one, Is.EqualTo(-Point.one));
            Assert.That(Point.one - Vector3.one, Is.EqualTo(Point.zero));
            Assert.That(Point.right - Vector3.left, Is.EqualTo(new Point(2, 0)));
            Assert.That(Point.up - Vector3.down, Is.EqualTo(new Point(0, 2)));
        }

        [Test]
        public void Operator_Multiply()
        {
            Assert.That(Point.zero * Point.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.one * Point.left, Is.EqualTo(Point.left));
            Assert.That(Point.one * new Point(5, -3), Is.EqualTo(new Point(5, -3)));
        }

        [Test]
        public void Operator_Multiply_Int()
        {
            Assert.That(Point.zero * 5, Is.EqualTo(Point.zero));
            Assert.That(Point.one * 2, Is.EqualTo(new Point(2, 2)));
            Assert.That(Point.one * -6, Is.EqualTo(new Point(-6, -6)));
        }

        [Test]
        public void Operator_Multiply_Vector2()
        {
            Assert.That(Point.zero * Vector2.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.one * Vector2.left, Is.EqualTo(Point.left));
            Assert.That(Point.one * new Vector2(5, -3), Is.EqualTo(new Point(5, -3)));
        }

        [Test]
        public void Operator_Multiply_Vector3()
        {
            Assert.That(Point.zero * Vector3.zero, Is.EqualTo(Point.zero));
            Assert.That(Point.one * Vector3.left, Is.EqualTo(Point.left));
            Assert.That(Point.one * new Vector3(5, -3), Is.EqualTo(new Point(5, -3)));
        }

        [Test]
        public void Operator_Divide()
        {
            Assert.That(Point.zero / Point.one, Is.EqualTo(Point.zero));
            Assert.That(new Point(8, 8) / new Point(2, 4), Is.EqualTo(new Point(4, 2)));
        }

        [Test]
        public void Operator_Divide_Int()
        {
            Assert.That(Point.zero / 1, Is.EqualTo(Point.zero));
            Assert.That(new Point(8, 8) / 2, Is.EqualTo(new Point(4, 4)));
        }

        [Test]
        public void Operator_Divide_Vector2()
        {
            Assert.That(Point.zero / Vector2.one, Is.EqualTo(Point.zero));
            Assert.That(new Point(8, 8) / new Vector2(2, 4), Is.EqualTo(new Point(4, 2)));
        }

        [Test]
        public void Operator_Divide_Vector3()
        {
            Assert.That(Point.zero / Vector2.one, Is.EqualTo(Point.zero));
            Assert.That(new Point(8, 8) / new Vector3(2, 4), Is.EqualTo(new Point(4, 2)));
        }

        [Test]
        public void Operator_Implicit_PointToVector2()
        {
            Vector2 vec = Point.one;
            Assert.That(vec, Is.EqualTo(Vector2.one));
        }

        [Test]
        public void Operator_Implicit_Vector2ToPoint()
        {
            Point p = Vector2.one;
            Assert.That(p, Is.EqualTo(Point.one));
        }

        [Test]
        public void Operator_Implicit_PointToVector3()
        {
            Vector3 vec = Point.one;
            Assert.That(vec, Is.EqualTo(new Vector3(1, 1, 0)));
        }

        [Test]
        public void Operator_Implicit_Vector3ToPoint()
        {
            Point p = Vector3.one;
            Assert.That(p, Is.EqualTo(Point.one));
        }
    }
}