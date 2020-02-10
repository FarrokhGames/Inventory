using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    [TestFixture]
    public class InventoryManagerTests
    {
        IInventoryItem CreateFullItem(int width, int height, bool canBeDropped = true)
        {
            var shape = new bool[width, height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    shape[x, y] = true;
                }
            }
            return new TestItem(null, new InventoryShape(shape), canBeDropped);
        }

        /*
        Constructor
        */

        [Test]
        public void CTOR_WidthAndHeightSet()
        {
            var inventory = new InventoryManager(new TestProvider(), 16, 8);
            Assert.That(inventory.width, Is.EqualTo(16));
            Assert.That(inventory.height, Is.EqualTo(8));
        }

        /*
        allItems
        */

        [Test]
        public void AllItems_ReturnsListOfAllItems()
        {
            var inventory = new InventoryManager(new TestProvider(), 16, 8);

            var item1 = CreateFullItem(1, 1);
            inventory.TryAdd(item1);

            var item2 = CreateFullItem(2, 2);
            inventory.TryAdd(item2);

            var allItems = inventory.allItems;
            Assert.That(allItems.Length, Is.EqualTo(2));
            Assert.That(allItems.Contains(item1), Is.True);
            Assert.That(allItems.Contains(item2), Is.True);
        }

        /*
        Contains
        */

        [TestCase(false, ExpectedResult = false)]
        [TestCase(true, ExpectedResult = true)]
        public bool Contains(bool doAdd)
        {
            var inventory = new InventoryManager(new TestProvider(), 16, 8);
            var item1 = CreateFullItem(1, 1);
            if (doAdd) { inventory.TryAdd(item1); }
            return inventory.Contains(item1);
        }

        /*
        IsFull
        */

        [Test]
        public void IsFull_Empty_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 16, 8);
            Assert.That(inventory.isFull, Is.False);
        }

        [Test]
        public void IsFull_NotFull_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 4, 4);
            inventory.TryAdd(CreateFullItem(2, 2));
            Assert.That(inventory.isFull, Is.False);
        }

        [Test]
        public void IsFull_Full_ReturnsTrue()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);
            inventory.TryAdd(CreateFullItem(2, 2));
            Assert.That(inventory.isFull, Is.True);
        }

        /*
        CanAdd
        */

        [Test]
        public void CanAdd_DoesFit_ReturnsTrue()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            Assert.That(inventory.CanAdd(CreateFullItem(1, 1)), Is.True);
            Assert.That(inventory.CanAdd(CreateFullItem(8, 4)), Is.True);
            Assert.That(inventory.CanAdd(CreateFullItem(3, 3)), Is.True);
        }

        [Test]
        public void CanAdd_DoesNotFit_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            Assert.That(inventory.CanAdd(CreateFullItem(1, 5)), Is.False);
            Assert.That(inventory.CanAdd(CreateFullItem(9, 1)), Is.False);
            Assert.That(inventory.CanAdd(CreateFullItem(9, 5)), Is.False);
        }

        [Test]
        public void CanAdd_ItemAlreadyAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            Assert.That(inventory.CanAdd(item), Is.False);
        }

        /*
        Add
        */

        [Test]
        public void Add_ItemDoesNotFit_NoItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);
            var item = CreateFullItem(3, 3);
            inventory.TryAdd(item);
            Assert.That(inventory.allItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void Add_ItemDoesFit_ItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(3, 3);
            inventory.TryAdd(item);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemAlreadyAdded_NoItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            inventory.TryAdd(item);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemAdded_OnItemAddedInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemAdded += (i) =>
            {
                callbacks++;
                lastItem = i;
            };

            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);

            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Add_ItemNotAdded_OnItemAddedNotInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var callbacks = 0;
            inventory.onItemAdded += (i) => { callbacks++; };
            inventory.TryAdd(CreateFullItem(2, 2));
            Assert.That(callbacks, Is.EqualTo(0));
        }

        /*
        CanAddAt
        */

        [Test]
        public void CanAddAt_DoesFit_ReturnsTrue()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 1), Vector2Int.one), Is.True);
            Assert.That(inventory.CanAddAt(CreateFullItem(8, 4), Vector2Int.zero), Is.True);
            Assert.That(inventory.CanAddAt(CreateFullItem(3, 3), Vector2Int.right), Is.True);
        }

        [Test]
        public void CanAddAt_DoesNotFit_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 5), Vector2Int.zero), Is.False);
            Assert.That(inventory.CanAddAt(CreateFullItem(9, 1), Vector2Int.one), Is.False);
        }

        [Test]
        public void CanAddAt_ItemInTheWay_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item1 = CreateFullItem(3, 1);
            inventory.TryAddAt(item1, Vector2Int.zero);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 1), new Vector2Int(1, 0)), Is.False);
        }

        [Test]
        public void CanAddAt_OutsideInventory_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            Assert.That(inventory.CanAddAt(CreateFullItem(1, 5), new Vector2Int(-1, 1)), Is.False);
            Assert.That(inventory.CanAddAt(CreateFullItem(9, 1), new Vector2Int(1, -1)), Is.False);
        }

        [Test]
        public void CanAddAt_ItemAlreadyAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 8, 4);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            Assert.That(inventory.CanAddAt(item, Vector2Int.zero), Is.False);
        }

        /*
        AddAt
        */

        [Test]
        public void AddAt_ItemDoesNotFit_NoItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);

            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemAddedFailed += (i) =>
            {
                callbacks++;
                lastItem = i;
            };
            
            var item = CreateFullItem(3, 3);
            inventory.TryAddAt(item, Vector2Int.zero);
            Assert.That(inventory.allItems.Count, Is.EqualTo(0));
            
            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemDoesFit_ItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(3, 3);
            inventory.TryAddAt(item, Vector2Int.zero);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemInTheWay_ItemNotAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);

            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemAddedFailed += (i) =>
            {
                callbacks++;
                lastItem = i;
            };
            
            var item1 = CreateFullItem(3, 1);
            inventory.TryAddAt(item1, Vector2Int.zero);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
            var item2 = CreateFullItem(1, 1);
            inventory.TryAddAt(item2, new Vector2Int(1, 0));
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
            
            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_OutsideInventory_ItemNotAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item1 = CreateFullItem(3, 1);
            inventory.TryAddAt(item1, new Vector2Int(-1, -1));
            Assert.That(inventory.allItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddAt_ItemAlreadyAdded_NoItemAdded()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            inventory.TryAddAt(item, Vector2Int.zero);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemAdded_ItemPositionUpdated()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(1, 1);
            inventory.TryAddAt(item, Vector2Int.one);
            Assert.That(item.position, Is.EqualTo(Vector2Int.one));
        }

        [Test]
        public void AddAt_ItemAdded_OnItemAddedInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemAdded += (i) =>
            {
                callbacks++;
                lastItem = i;
            };

            var item = CreateFullItem(1, 1);
            inventory.TryAddAt(item, Vector2Int.zero);

            Assert.That(lastItem, Is.Not.Null);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void AddAt_ItemNotAdded_OnItemAddedNotInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var callbacks = 0;
            inventory.onItemAdded += (i) => { callbacks++; };
            inventory.TryAddAt(CreateFullItem(2, 2), Vector2Int.zero);
            Assert.That(callbacks, Is.EqualTo(0));
        }

        /*
        CanRemove
        */

        [Test]
        public void CanRemove_ItemNotAdded_ReturnsFalse()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            Assert.That(inventory.CanRemove(CreateFullItem(1, 1)), Is.False);
        }

        [Test]
        public void CanRemove_ItemAdded_ReturnsTrue()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            Assert.That(inventory.CanRemove(item), Is.True);
        }

        /*
        Remove
        */

        [Test]
        public void Remove_ItemAddedFirst_ItemRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.TryRemove(item);
            Assert.That(inventory.Contains(item), Is.False);
        }

        [Test]
        public void Remove_ItemNotAdded_OnItemRemovedNotInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var callbacks = 0;
            inventory.onItemRemoved += (i) => { callbacks++; };
            inventory.TryRemove(CreateFullItem(1, 1));
            Assert.That(callbacks, Is.EqualTo(0));
        }

        [Test]
        public void Remove_ItemAdded_OnItemRemovedInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemRemoved += (i) =>
            {
                lastItem = i;
                callbacks++;
            };
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            inventory.TryRemove(item);
            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        /*
        Drop
        */

        [Test]
        public void Drop_ItemNotPresentInInventory_OnItemDroppedNotInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);

            var callbacks = 0;
            inventory.onItemDropped += (i) =>
            {
                callbacks++;
            };

            var item = CreateFullItem(1, 1);
            inventory.TryDrop(item);

            Assert.That(callbacks, Is.EqualTo(0));
        }

        [Test]
        public void Drop_ItemPresentInInventory_ItemRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.TryDrop(item);
            Assert.That(inventory.Contains(item), Is.False);
        }
        
        [Test]
        public void Drop_CantDrop_ItemNotRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            
            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemDroppedFailed += (i) =>
            {
                lastItem = i;
                callbacks++;
            };
            
            var item = CreateFullItem(1, 1, false);
            inventory.TryAdd(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.TryDrop(item);
            Assert.That(inventory.Contains(item), Is.True);
            
            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Drop_ItemPresentInInventory_OnItemDroppedInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);

            var callbacks = 0;
            IInventoryItem lastItem = null;
            inventory.onItemDropped += (i) =>
            {
                lastItem = i;
                callbacks++;
            };

            var item = CreateFullItem(1, 1);
            inventory.TryAdd(item);
            inventory.TryDrop(item);

            Assert.That(lastItem, Is.SameAs(item));
            Assert.That(callbacks, Is.EqualTo(1));
        }

        /*
        DropAll
        */

        [Test]
        public void DropAll_AllItemsRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.TryAdd(item1);
            inventory.TryAdd(item2);
            inventory.TryAdd(item3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));
            inventory.DropAll();
            Assert.That(inventory.allItems.Count, Is.EqualTo(0));
            Assert.That(inventory.Contains(item1), Is.False);
            Assert.That(inventory.Contains(item2), Is.False);
            Assert.That(inventory.Contains(item3), Is.False);
        }

        [Test]
        public void DropAll_OnItemDroppedInvokedForAllItems()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);

            var droppedItems = new List<IInventoryItem>();
            inventory.onItemDropped += (i) =>
            {
                droppedItems.Add(i);
            };

            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.TryAdd(item1);
            inventory.TryAdd(item2);
            inventory.TryAdd(item3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));

            inventory.DropAll();

            Assert.That(droppedItems.Count, Is.EqualTo(3));
            Assert.That(droppedItems.Contains(item1), Is.True);
            Assert.That(droppedItems.Contains(item2), Is.True);
            Assert.That(droppedItems.Contains(item3), Is.True);
        }

        /*
        Clear
        */

        [Test]
        public void Clear_AllItemsRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item = CreateFullItem(3, 3);
            inventory.TryAdd(item);
            Assert.That(inventory.Contains(item), Is.True);
            inventory.Clear();
            Assert.That(inventory.Contains(item), Is.False);
        }

        /*
        GetAtPoint
        */

        [Test]
        public void GetAtPoint_ItemFound_ReturnsItem()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var item1 = CreateFullItem(2, 2);
            var item2 = CreateFullItem(1, 2);
            var item3 = CreateFullItem(1, 1);
            inventory.TryAdd(item1);
            inventory.TryAdd(item2);
            inventory.TryAdd(item3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));

            Assert.That(inventory.GetAtPoint(new Vector2Int(0, 0)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Vector2Int(1, 0)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Vector2Int(1, 1)), Is.EqualTo(item1));
            Assert.That(inventory.GetAtPoint(new Vector2Int(0, 1)), Is.EqualTo(item1));

            Assert.That(inventory.GetAtPoint(new Vector2Int(2, 0)), Is.EqualTo(item2));
            Assert.That(inventory.GetAtPoint(new Vector2Int(2, 1)), Is.EqualTo(item2));

            Assert.That(inventory.GetAtPoint(new Vector2Int(0, 2)), Is.EqualTo(item3));

            Assert.That(inventory.GetAtPoint(new Vector2Int(2, 2)), Is.Null);
            Assert.That(inventory.GetAtPoint(new Vector2Int(2, 3)), Is.Null);
        }

        [Test]
        public void GetAtPoint_ItemNotFoundReturnsNull()
        {
            var inventory = new InventoryManager(new TestProvider(), 1, 1);
            Assert.That(inventory.GetAtPoint(new Vector2Int(1, 1)), Is.Null);
        }

        /*
        GetAtPoint
        */

        [Test]
        public void Resize_WidthAndHeightUpdated()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);
            inventory.Resize(6, 8);
            Assert.That(inventory.width, Is.EqualTo(6));
            Assert.That(inventory.height, Is.EqualTo(8));
        }

        [Test]
        public void Resize_OnResizedInvoked()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);
            var callbacks = 0;
            inventory.onResized += () => { callbacks++; };
            inventory.Resize(3, 3);
            Assert.That(callbacks, Is.EqualTo(1));
        }

        [Test]
        public void Resize_AllItemFits_NoItemsRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 2, 2);
            var item1 = CreateFullItem(1, 1);
            var item2 = CreateFullItem(2, 1);
            var item3 = CreateFullItem(1, 1);
            inventory.TryAdd(item1);
            inventory.TryAdd(item2);
            inventory.TryAdd(item3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));
            inventory.Resize(3, 3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));
            Assert.That(inventory.allItems.Contains(item1), Is.True);
            Assert.That(inventory.allItems.Contains(item2), Is.True);
            Assert.That(inventory.allItems.Contains(item3), Is.True);
        }

        [Test]
        public void Resize_SomeItemsNoLongerFits_ItemsRemoved()
        {
            var inventory = new InventoryManager(new TestProvider(), 3, 3);
            var droppedItems = new List<IInventoryItem>();
            inventory.onItemDropped += (i) =>
            {
                droppedItems.Add(i);
            };
            var item1 = CreateFullItem(3, 1);
            var item2 = CreateFullItem(1, 1);
            var item3 = CreateFullItem(2, 2);
            inventory.TryAdd(item1);
            inventory.TryAdd(item2);
            inventory.TryAdd(item3);
            Assert.That(inventory.allItems.Count, Is.EqualTo(3));
            inventory.Resize(2, 2);
            Assert.That(inventory.allItems.Count, Is.EqualTo(1));
            Assert.That(inventory.allItems.Contains(item1), Is.False);
            Assert.That(inventory.allItems.Contains(item2), Is.True);
            Assert.That(inventory.allItems.Contains(item3), Is.False);
            Assert.That(droppedItems.Count, Is.EqualTo(2));
            Assert.That(droppedItems.Contains(item1), Is.True);
            Assert.That(droppedItems.Contains(item3), Is.True);
        }
    }
}