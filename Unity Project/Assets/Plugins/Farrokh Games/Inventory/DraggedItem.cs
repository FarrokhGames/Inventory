using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Class for keeping track of dragged items
    /// </summary>
    public class DraggedItem
    {
        public enum DropMode
        {
            Added,
            Swapped,
            Returned,
            Dropped,
        }

        /// <summary>
        /// Returns the InventoryController this item originated from
        /// </summary>
        public InventoryController originalController { get; private set; }

        /// <summary>
        /// Returns the point inside the inventory from which this item originated from
        /// </summary>
        public Vector2Int originPoint { get; private set; }

        /// <summary>
        /// Returns the item-instance that is being dragged
        /// </summary>
        public IInventoryItem item { get; private set; }

        /// <summary>
        /// Gets or sets the InventoryController currently in control of this item
        /// </summary>
        public InventoryController currentController;

        private readonly RectTransform _canvasTransform;
        private readonly Image _image;
        private Vector2 _offset;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="canvasTransform">The rectTransform of the canvas</param>
        /// <param name="originalController">The InventoryController this item originated from</param>
        /// <param name="originPoint">The point inside the inventory from which this item originated from</param>
        /// <param name="item">The item-instance that is being dragged</param>
        /// <param name="offset">The starting offset of this item</param>
        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        public DraggedItem(
            RectTransform canvasTransform,
            InventoryController originalController,
            Vector2Int originPoint,
            IInventoryItem item,
            Vector2 offset)
        {
            this.originalController = originalController;
            currentController = this.originalController;
            this.originPoint = originPoint;
            this.item = item;

            _canvasTransform = canvasTransform;
            _offset = offset;

            // Create an image representing the dragged item
            _image = new GameObject("DraggedItem").AddComponent<Image>();
            _image.raycastTarget = false;
            _image.transform.SetParent(_canvasTransform);
            _image.transform.SetAsLastSibling();
            _image.transform.localScale = Vector3.one;
            _image.sprite = item.sprite;
            _image.SetNativeSize();
        }

        /// <summary>
        /// Gets or sets the position of this dragged item
        /// </summary>
        public Vector2 position
        {
            set
            {
                // Move the image
                _image.rectTransform.localPosition = (value - (_canvasTransform.sizeDelta * 0.5f)) + _offset;

                // Make selections
                if (currentController != null)
                {
                    item.position = currentController.ScreenToGrid(value + _offset + currentController.GetDraggedItemOffset(item));
                    var canAdd = currentController.inventory.CanAddAt(item, item.position) || CanSwap();
                    currentController.inventoryRenderer.SelectItem(item, !canAdd, Color.white);
                }

                // Slowly animate the item towards the center of the mouse pointer
                _offset = Vector2.Lerp(_offset, Vector2.zero, Time.deltaTime * 10f);
            }
        }

        /// <summary>
        /// Drop this item at the given position
        /// </summary>
        public DropMode Drop(Vector2 pos)
        {
            DropMode mode;
            if (currentController != null)
            {
                var grid = currentController.ScreenToGrid(pos + _offset + currentController.GetDraggedItemOffset(item));

                // Try to add new item
                if (currentController.inventory.CanAddAt(item, grid))
                {
                    currentController.inventory.TryAddAt(item, grid); // Place the item in a new location
                    mode = DropMode.Added;
                }
                // Adding did not work, try to swap
                else if (CanSwap())
                {
                    var otherItem = currentController.inventory.allItems[0];
                    currentController.inventory.TryRemove(otherItem);
                    originalController.inventory.TryAdd(otherItem);
                    currentController.inventory.TryAdd(item);
                    mode = DropMode.Swapped;
                }
                // Could not add or swap, return the item
                else
                {
                    originalController.inventory.TryAddAt(item, originPoint); // Return the item to its previous location
                    mode = DropMode.Returned;

                }

                currentController.inventoryRenderer.ClearSelection();
            }
            else
            {
                mode = DropMode.Dropped;
                originalController.inventory.TryDrop(item); // Drop the item
            }

            // Destroy the image representing the item
            Object.Destroy(_image.gameObject);

            return mode;
        }

        /* 
         * Returns true if its possible to swap
         */
        private bool CanSwap()
        {
            if (!currentController.inventory.CanSwap(item)) return false;
            var otherItem = currentController.inventory.allItems[0];
            return originalController.inventory.CanAdd(otherItem) && currentController.inventory.CanRemove(otherItem);
        }
    }
}