using System;
using FarrokhGames.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    public interface IInventoryController
    {
        Action<IInventoryItem> OnItemHovered { get; set; }
        Action<IInventoryItem> OnItemPickedUp { get; set; }
        Action<IInventoryItem> OnItemAdded { get; set; }
        Action<IInventoryItem> OnItemSwapped { get; set; }
        Action<IInventoryItem> OnItemReturned { get; set; }
        Action<IInventoryItem> OnItemDropped { get; set; }
    }

    /// <summary>
    /// Enables human interaction with an inventory renderer using Unity's event systems
    /// </summary>
    [RequireComponent(typeof(InventoryRenderer))]
    public class InventoryController : MonoBehaviour,
        IPointerDownHandler, IBeginDragHandler, IDragHandler,
        IEndDragHandler, IPointerExitHandler, IPointerEnterHandler,
        IInventoryController
        {
            // The dragged item is static and shared by all controllers
            // This way items can be moved between controllers easily
            private static DraggedItem _draggedItem = null;

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemHovered { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemPickedUp { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemAdded { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemSwapped { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemReturned { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> OnItemDropped { get; set; }

            private Canvas _canvas;
            private InventoryRenderer _renderer;
            private IInventoryManager _inventory { get { return _renderer._inventory; } }

            private IInventoryItem _itemToDrag;
            private PointerEventData _currentEventData = null;
            private IInventoryItem _lastHoveredItem = null;

            /*
             * Setup
             */
            void Awake()
            {
                _renderer = GetComponent<InventoryRenderer>();
                if (_renderer == null) { throw new NullReferenceException("Could not find a renderer. This is not allowed!"); }

                // Find the canvas
                var canvases = GetComponentsInParent<Canvas>();
                if (canvases.Length == 0) { throw new NullReferenceException("Could not find a canvas."); }
                _canvas = canvases[canvases.Length - 1];
            }

            /*
             * Grid was clicked (IPointerDownHandler)
             */
            public void OnPointerDown(PointerEventData eventData)
            {
                if (_draggedItem == null)
                {
                    // Get which item to drag (item will be null of none were found)
                    var grid = ScreenToGrid(eventData.position);
                    _itemToDrag = _inventory.GetAtPoint(grid);
                }
            }

            /*
             * Dragging started (IBeginDragHandler)
             */
            public void OnBeginDrag(PointerEventData eventData)
            {
                _renderer.ClearSelection();

                if (_itemToDrag != null && _draggedItem == null)
                {
                    var localPosition = ScreenToLocalPositionInRenderer(eventData.position);
                    var itemOffest = _renderer.GetItemOffset(_itemToDrag);
                    var offset = itemOffest - localPosition;

                    // Create a dragged item 
                    _draggedItem = new DraggedItem(
                        _canvas,
                        this,
                        _itemToDrag.Position,
                        _itemToDrag,
                        offset
                    );

                    // Remove the item from inventory
                    _inventory.TryRemove(_itemToDrag);

                    if (OnItemPickedUp != null) { OnItemPickedUp(_itemToDrag); }
                }
            }

            /*
             * Dragging is continuing (IDragHandler)
             */
            public void OnDrag(PointerEventData eventData)
            {
                _currentEventData = eventData;
                if (_draggedItem != null)
                {
                    // Update the items position
                    //_draggedItem.Position = eventData.position;
                }
            }

            /*
             * Dragging stopped (IEndDragHandler)
             */
            public void OnEndDrag(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    var mode = _draggedItem.Drop(eventData.position);

                    switch (mode)
                    {
                        case DraggedItem.DropMode.Added:
                            if (OnItemAdded != null) { OnItemAdded(_itemToDrag); }
                            break;
                        case DraggedItem.DropMode.Swapped:
                            if (OnItemSwapped != null) { OnItemSwapped(_itemToDrag); }
                            break;
                        case DraggedItem.DropMode.Returned:
                            if (OnItemReturned != null) { OnItemReturned(_itemToDrag); }
                            break;
                        case DraggedItem.DropMode.Dropped:
                            if (OnItemDropped != null) { OnItemDropped(_itemToDrag); }
                            ClearHoveredItem();
                            break;
                    }

                    _draggedItem = null;
                }
            }

            /*
             * Pointer left the inventory (IPointerExitHandler)
             */
            public void OnPointerExit(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    // Clear the item as it leaves its current controller
                    _draggedItem.CurrentController = null;
                    _renderer.ClearSelection();
                }
                else { ClearHoveredItem(); }
                _currentEventData = null;
            }

            /*
             * Pointer entered the inventory (IPointerEnterHandler)
             */
            public void OnPointerEnter(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    // Change which controller is in control of the dragged item
                    _draggedItem.CurrentController = this;
                }
                _currentEventData = eventData;
            }

            /*
             * Update loop
             */
            void Update()
            {
                if (_currentEventData != null)
                {
                    if (_draggedItem == null)
                    {
                        // Detect hover
                        var grid = ScreenToGrid(_currentEventData.position);
                        var item = _inventory.GetAtPoint(grid);
                        if (item != _lastHoveredItem)
                        {
                            if (OnItemHovered != null) { OnItemHovered(item); }
                            _lastHoveredItem = item;
                        }
                    }
                    else
                    {
                        // Update position while dragging
                        _draggedItem.Position = _currentEventData.position;
                    }
                }
            }

            /* 
             * 
             */
            private void ClearHoveredItem()
            {
                if (_lastHoveredItem != null && OnItemHovered != null)
                {
                    OnItemHovered(null);
                }
                _lastHoveredItem = null;
            }

            /*
             * Get a point on the grid from a given screen point
             */
            private Vector2Int ScreenToGrid(Vector2 screenPoint)
            {
                var pos = ScreenToLocalPositionInRenderer(screenPoint);
                pos.x += _renderer.RectTransform.sizeDelta.x / 2;
                pos.y += _renderer.RectTransform.sizeDelta.y / 2;
                return new Vector2Int(Mathf.FloorToInt(pos.x / _renderer.CellSize.x), Mathf.FloorToInt(pos.y / _renderer.CellSize.y));
            }

            /*
             * Returns the offset between dragged item and the grid 
             */
            private Vector2 GetDraggedItemOffset(IInventoryItem item)
            {
                var gx = -((item.Width * _renderer.CellSize.x) / 2f) + (_renderer.CellSize.x / 2);
                var gy = -((item.Height * _renderer.CellSize.y) / 2f) + (_renderer.CellSize.y / 2);
                return new Vector2(gx, gy);
            }

            private Vector2 ScreenToLocalPositionInRenderer(Vector2 screenPosition)
            {
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _renderer.RectTransform,
                    screenPosition,
                    _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                    out localPosition
                );
                return localPosition;
            }

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
                public InventoryController OriginalController { get; private set; }

                /// <summary>
                /// Returns the point inside the inventory from which this item originated from
                /// </summary>
                public Vector2Int OriginPoint { get; private set; }

                /// <summary>
                /// Returns the item-instance that is being dragged
                /// </summary>
                public IInventoryItem Item { get; private set; }

                /// <summary>
                /// Gets or sets the InventoryController currently in control of this item
                /// </summary>
                public InventoryController CurrentController;

                private RectTransform _canvasTransform;
                private Image _image;
                private Vector2 _offset;

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="originalController">The InventoryController this item originated from</param>
                /// <param name="originPoint">The point inside the inventory from which this item originated from</param>
                /// <param name="item">The item-instance that is being dragged</param>
                /// <param name="offset">The starting offset of this item</param>
                public DraggedItem(
                    Canvas canvas,
                    InventoryController originalController,
                    Vector2Int originPoint,
                    IInventoryItem item,
                    Vector2 offset)
                {
                    OriginalController = originalController;
                    CurrentController = OriginalController;
                    OriginPoint = originPoint;
                    Item = item;

                    _canvasTransform = canvas.transform as RectTransform;;
                    _offset = offset;

                    // Create an image representing the dragged item
                    _image = new GameObject("DraggedItem").AddComponent<Image>();
                    _image.raycastTarget = false;
                    _image.transform.SetParent(_canvasTransform);
                    _image.transform.SetAsLastSibling();
                    _image.transform.localScale = Vector3.one;
                    _image.sprite = item.Sprite;
                    _image.SetNativeSize();
                }

                /// <summary>
                /// Gets or sets the position of this dragged item
                /// </summary>
                public Vector2 Position
                {
                    get { return _image.rectTransform.position; }
                    set
                    {
                        // Move the image
                        _image.rectTransform.localPosition = (value - (_canvasTransform.sizeDelta * 0.5f)) + _offset;

                        // Make selections
                        if (CurrentController != null)
                        {
                            _draggedItem.Item.Position = CurrentController.ScreenToGrid(value + _offset + CurrentController.GetDraggedItemOffset(_draggedItem.Item));
                            var canAdd = CurrentController._inventory.CanAddAt(_draggedItem.Item, _draggedItem.Item.Position) || CanSwap();
                            CurrentController._renderer.SelectItem(_draggedItem.Item, !canAdd, Color.white);
                        }

                        // Slowly animate the item towards the center of the mouse pointer
                        _offset = Vector2.Lerp(_offset, Vector2.zero, Time.deltaTime * 10f);
                    }
                }

                /// <summary>
                /// Drop this item at the given position
                /// </summary>
                /// <param name="position">The position at which to drop the item</param>
                public DropMode Drop(Vector2 position)
                {
                    DropMode mode;
                    if (CurrentController != null)
                    {
                        var grid = CurrentController.ScreenToGrid(position + _offset + CurrentController.GetDraggedItemOffset(Item));

                        // Try to add new item
                        if (CurrentController._inventory.CanAddAt(Item, grid))
                        {
                            CurrentController._inventory.TryAddAt(Item, grid); // Place the item in a new location
                            mode = DropMode.Added;
                        }
                        // Adding did not work, try to swap
                        else if (CanSwap())
                        {
                            var otherItem = CurrentController._inventory.AllItems[0];
                            CurrentController._inventory.TryRemove(otherItem);
                            OriginalController._inventory.TryAdd(otherItem);
                            CurrentController._inventory.TryAdd(Item);
                            mode = DropMode.Swapped;
                        }
                        // Could not add or swap, return the item
                        else
                        {
                            OriginalController._inventory.TryAddAt(Item, OriginPoint); // Return the item to its previous location
                            mode = DropMode.Returned;

                        }

                        CurrentController._renderer.ClearSelection();
                    }
                    else
                    {
                        mode = DropMode.Dropped;
                        OriginalController._inventory.TryDrop(_draggedItem.Item); // Drop the item
                    }

                    // Destroy the image representing the item
                    GameObject.Destroy(_image.gameObject);

                    return mode;
                }

                /* 
                 * Returns true if its possible to swap
                 */
                private bool CanSwap()
                {
                    if (CurrentController._inventory.CanSwap(Item))
                    {
                        var otherItem = CurrentController._inventory.AllItems[0];
                        if (OriginalController._inventory.CanAdd(otherItem) && CurrentController._inventory.CanRemove(otherItem))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
        }
}