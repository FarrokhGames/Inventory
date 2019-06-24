using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FarrokhGames.Inventory.Internal
{
    public abstract class AbstractInventoryManager : IInventoryManager, IDisposable
    {
        private Vector2Int _size = Vector2Int.one;
        private IInventoryProvider _provider;
        private IInventoryItem[] _itemCache = null;
        private Rect _fullRect;

        public AbstractInventoryManager(IInventoryProvider provider)
        {
            _provider = provider;
            Rebuild();
        }

        public int Width { get { return _size.x; } }
        public int Height { get { return _size.y; } }

        protected void Resize(int width, int height)
        {
            _size.x = width;
            _size.y = height;
            RebuildRect();
        }

        protected Rect Rect { get { return _fullRect; } }

        private void RebuildRect()
        {
            _fullRect = new Rect(0, 0, _size.x, _size.y);
            HandleSizeChanged();
            if (OnResized != null)OnResized();
        }

        protected abstract void HandleSizeChanged();

        public void Rebuild()
        {
            Rebuild(false);
        }

        private void Rebuild(bool silent)
        {
            _itemCache = new IInventoryItem[_provider.InventoryItemCount];
            for (var i = 0; i < _provider.InventoryItemCount; i++)
            {
                _itemCache[i] = _provider.GetInventoryItem(i);
            }
            if (!silent && OnRebuilt != null)OnRebuilt();
        }

        public void Dispose()
        {
            _provider = null;
            _itemCache = null;
        }

        public IInventoryItem[] AllItems { get { return _itemCache; } }

        public Action OnRebuilt { get; set; }
        public Action<IInventoryItem> OnItemDropped { get; set; }
        public Action<IInventoryItem> OnItemAdded { get; set; }
        public Action<IInventoryItem> OnItemRemoved { get; set; }
        public Action OnResized { get; set; }

        public abstract bool IsFull { get; }

        public virtual IInventoryItem GetAtPoint(Vector2Int point)
        {
            foreach (var item in _itemCache)
            {
                if (item.Contains(point)) { return item; }
            }
            return null;
        }

        public bool Remove(IInventoryItem item)
        {
            if (CanRemove(item))
            {
                var success = _provider.RemoveInventoryItem(item);
                if (success)
                {
                    Rebuild(true);
                    if (OnItemRemoved != null)OnItemRemoved(item);
                }

                return success;
            }
            return false;
        }

        public bool Drop(IInventoryItem item)
        {
            if (CanDrop(item))
            {
                var success = _provider.DropInventoryItem(item);
                if (success)
                {
                    Rebuild(true);
                    if (OnItemDropped != null)OnItemDropped(item);
                }
                return success;
            }
            return false;
        }

        public virtual bool CanAddAt(IInventoryItem item, Vector2Int point)
        {
            return _provider.CanAddInventoryItem(item);
        }

        public virtual bool AddAt(IInventoryItem item, Vector2Int Point)
        {
            if (CanAddAt(item, Point))
            {
                var sucess = _provider.AddInventoryItem(item);
                if (sucess)
                {
                    item.Position = Point;
                    Rebuild(true);
                    if (OnItemAdded != null)OnItemAdded(item);
                }
                return sucess;
            }
            return false;
        }

        public abstract bool CanAdd(IInventoryItem item);

        public abstract bool Add(IInventoryItem item);

        /// <inheritdoc />
        public void DropAll()
        {
            var itemsToDrop = AllItems.ToArray();
            foreach (var item in itemsToDrop)
            {
                Drop(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            var itemsToRemove = AllItems.ToArray();
            foreach (var item in AllItems)
            {
                Remove(item);
            }
        }

        public bool Contains(IInventoryItem item)
        {
            return AllItems.Contains(item);
        }

        public bool CanRemove(IInventoryItem item)
        {
            return Contains(item) && _provider.CanRemoveInventoryItem(item);
        }

        public bool CanDrop(IInventoryItem item)
        {
            return Contains(item) && _provider.CanDropInventoryItem(item);
        }
    }
}