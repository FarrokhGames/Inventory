using System;
using NUnit.Framework;

namespace FarrokhGames.Shared
{
    [TestFixture]
    public class PoolTests
    {
        private class PoolObject { }

        private int _creatorCount;

        private PoolObject Creator()
        {
            _creatorCount++;
            return new PoolObject();
        }

        [SetUp]
        public void Setup_PoolTests()
        {
            _creatorCount = 0;
        }

        [Test]
        public void CTOR_NullCreator_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Pool<PoolObject>(null));
        }

        [Test]
        public void CTOR_InitialCountLessThanZero_ArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Pool<PoolObject>(Creator, -1));
        }

        [Test]
        public void CTOR_InitialCount_CreatesSameNumberOfObjects()
        {
            new Pool<PoolObject>(Creator, 5, true);
            Assert.That(_creatorCount, Is.EqualTo(5));
        }

        [Test]
        public void Count_ReturnsRightNumberOfObjects()
        {
            var pool = new Pool<PoolObject>(Creator, 8, true);
            Assert.That(pool.Count, Is.EqualTo(8));
            var toRecycele = pool.Take();
            pool.Take();
            Assert.That(pool.Count, Is.EqualTo(6));
            pool.Recycle(toRecycele);
            Assert.That(pool.Count, Is.EqualTo(7));
        }

        [TestCase(5, ExpectedResult = false)]
        [TestCase(0, ExpectedResult = true)]
        public bool IsEmpty_Success(int initialCount)
        {
            var pool = new Pool<PoolObject>(Creator, initialCount);
            return pool.IsEmpty;
        }

        [TestCase(false, ExpectedResult = false)]
        [TestCase(true, ExpectedResult = true)]
        public bool CanTake_AllowedToTakeWhenEmpty_Success(bool allowTakingWhenEmpty)
        {
            var pool = new Pool<PoolObject>(Creator, 0, allowTakingWhenEmpty);
            return pool.CanTake;
        }

        [Test]
        public void Take_Empty_ReturnsNull()
        {
            var pool = new Pool<PoolObject>(Creator, 0, false);
            Assert.That(pool.Take(), Is.Null);
        }

        [Test]
        public void Take_FromInitialCount_NoNewObjectsCreated()
        {
            var pool = new Pool<PoolObject>(Creator, 5, false);
            _creatorCount = 0;
            Assert.That(pool.Take(), Is.Not.Null);
            Assert.That(_creatorCount, Is.Zero);
        }

        [Test]
        public void Take_Empty_NewObjectsCreated()
        {
            var pool = new Pool<PoolObject>(Creator, 2, true);
            _creatorCount = 0;
            pool.Take();
            pool.Take();
            Assert.That(pool.Take(), Is.Not.Null);
            Assert.That(pool.Take(), Is.Not.Null);
            Assert.That(pool.Take(), Is.Not.Null);
            Assert.That(_creatorCount, Is.EqualTo(3));
        }

        [Test]
        public void Take_ObjectAreNotTheSame()
        {
            var pool = new Pool<PoolObject>(Creator, 3, false);
            var obj1 = pool.Take();
            var obj2 = pool.Take();
            var obj3 = pool.Take();
            Assert.That(obj1, Is.Not.SameAs(obj2));
            Assert.That(obj2, Is.Not.SameAs(obj3));
            Assert.That(obj3, Is.Not.SameAs(obj1));
        }

        [Test]
        public void Recycle_NotPartOfPool_InvalidOperationException()
        {
            var pool = new Pool<PoolObject>(Creator, 2, true);
            Assert.Throws<InvalidOperationException>(() => pool.Recycle(new PoolObject()));
        }

        [Test]
        public void Recycle_ObjectReturnedToThePool()
        {
            var pool = new Pool<PoolObject>(Creator, 2, true);
            _creatorCount = 0;
            var obj = pool.Take();
            Assert.That(pool.Count, Is.EqualTo(1));
            pool.Recycle(obj);
            Assert.That(pool.Count, Is.EqualTo(2));
            var obj1 = pool.Take();
            var obj2 = pool.Take();
            var obj3 = pool.Take();
            Assert.That(_creatorCount, Is.EqualTo(1));
            pool.Recycle(obj1);
            Assert.That(pool.Count, Is.EqualTo(1));
            pool.Recycle(obj2);
            Assert.That(pool.Count, Is.EqualTo(2));
            pool.Recycle(obj3);
            Assert.That(pool.Count, Is.EqualTo(3));
        }

        [Test]
        public void GetInactive_ReturnsListOfUnactiveObjects()
        {
            var pool = new Pool<PoolObject>(Creator, 5, true);
            Assert.That(pool.GetInactive().Count, Is.EqualTo(5));
            var obj = pool.Take();
            Assert.That(pool.GetInactive().Count, Is.EqualTo(4));
            pool.Recycle(obj);
            Assert.That(pool.GetInactive().Count, Is.EqualTo(5));
        }

        [Test]
        public void GetInactive_ReturnsCopies()
        {
            var pool = new Pool<PoolObject>(Creator, 5, true);
            var inactive1 = pool.GetInactive();
            var inactive2 = pool.GetInactive();
            Assert.That(inactive1, Is.Not.SameAs(inactive2));
        }

        [Test]
        public void GetActive_ReturnsListOfUnactiveObjects()
        {
            var pool = new Pool<PoolObject>(Creator, 1, true);
            Assert.That(pool.GetActive().Count, Is.EqualTo(0));
            var obj = pool.Take();
            Assert.That(pool.GetActive().Count, Is.EqualTo(1));
            pool.Recycle(obj);
            Assert.That(pool.GetActive().Count, Is.EqualTo(0));
        }

        [Test]
        public void GetActive_ReturnsCopies()
        {
            var pool = new Pool<PoolObject>(Creator, 1, true);
            var active1 = pool.GetActive();
            var active2 = pool.GetActive();
            Assert.That(active1, Is.Not.SameAs(active2));
        }
    }
}