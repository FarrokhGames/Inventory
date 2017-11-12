using System;
using FarrokhGames.Shared;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Enables human interaction with an inventory renderer using Unity's event systems
    /// </summary>
    [RequireComponent(typeof(InventoryRenderer))]
    public class InventoryController : MonoBehaviour,
        IPointerDownHandler, IBeginDragHandler, IDragHandler,
        IEndDragHandler, IPointerExitHandler, IPointerEnterHandler
        {
            // The dragged item is static and shared by all controllers
            // This way items can be moved between controllers easily
            private static DraggedItem _draggedItem = null;

            private InventoryRenderer _renderer;
            private InventoryManager _inventory { get { return _renderer._inventory; } }
            private IInventoryItem _itemToDrag;

            /*
             * Setup
             */
            void Awake()
            {
                _renderer = GetComponent<InventoryRenderer>();
                if (_renderer == null) { throw new NullReferenceException("Could not find a renderer. This is not allowed!"); }
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
                    var t = _renderer.RectTransform.InverseTransformPoint(eventData.position);
                    var localPosition = new Vector2(t.x, t.y);
                    var offset = _renderer.GetItemOffset(_itemToDrag) - localPosition;

                    // Create a dragged item 
                    _draggedItem = new DraggedItem(
                        this,
                        _itemToDrag.Shape.Position,
                        _itemToDrag,
                        offset
                    );

                    // Remove the item from inventory
                    _inventory.Remove(_itemToDrag);
                }
            }

            /*
             * Dragging is continuing (IDragHandler)
             */
            public void OnDrag(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    // Update the items position
                    _draggedItem.Position = eventData.position;
                }
            }

            /*
             * Dragging stopped (IEndDragHandler)
             */
            public void OnEndDrag(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    _draggedItem.Drop(eventData.position);
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
            }

            /*
             * Get a point on the grid from a given screen point
             */
            private Vector2Int ScreenToGrid(Vector2 screenPoint)
            {
                var pos = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _renderer.RectTransform,
                    screenPoint,
                    null,
                    out pos
                );
                pos.x += _renderer.RectTransform.sizeDelta.x / 2;
                pos.y += _renderer.RectTransform.sizeDelta.y / 2;
                return new Vector2Int(Mathf.FloorToInt(pos.x / _renderer.CellSize.x), Mathf.FloorToInt(pos.y / _renderer.CellSize.y));
            }

            /*
             * Returns the offset between dragged item and the grid 
             */
            private Vector2 GetDraggedItemOffset(IInventoryItem item)
            {
                var gx = -((item.Shape.Width * _renderer.CellSize.x) / 2f) + (_renderer.CellSize.x / 2);
                var gy = -((item.Shape.Height * _renderer.CellSize.y) / 2f) + (_renderer.CellSize.y / 2);
                return new Vector2(gx, gy);
            }

            /// <summary>
            /// Class for keeping track of dragged items
            /// </summary>
            public class DraggedItem
            {
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

                private Image _image;
                Vector2 _offset;

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="originalController">The InventoryController this item originated from</param>
                /// <param name="originPoint">The point inside the inventory from which this item originated from</param>
                /// <param name="item">The item-instance that is being dragged</param>
                /// <param name="offset">The starting offset of this item</param>
                public DraggedItem(
                    InventoryController originalController,
                    Vector2Int originPoint,
                    IInventoryItem item,
                    Vector2 offset)
                {
                    OriginalController = originalController;
                    CurrentController = OriginalController;
                    OriginPoint = originPoint;
                    Item = item;
                    _offset = offset;

                    // Create an image representing the dragged item
                    _image = new GameObject("DraggedItem").AddComponent<Image>();
                    _image.raycastTarget = false;
                    _image.transform.SetParent(originalController.gameObject.GetComponentInParent<Canvas>().transform);
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
                        _image.rectTransform.position = value + _offset;

                        // Make selections
                        if (CurrentController != null)
                        {
                            _draggedItem.Item.Shape.Position = CurrentController.ScreenToGrid(value + _offset + CurrentController.GetDraggedItemOffset(_draggedItem.Item));
                            var canAdd = CurrentController._inventory.CanAddAt(_draggedItem.Item, _draggedItem.Item.Shape.Position);
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
                public void Drop(Vector2 position)
                {
                    if (CurrentController != null)
                    {
                        var grid = CurrentController.ScreenToGrid(position + _offset + CurrentController.GetDraggedItemOffset(Item));
                        if (CurrentController._inventory.CanAddAt(Item, grid))
                        {
                            CurrentController._inventory.AddAt(Item, grid); // Place the item in a new location
                        }
                        else
                        {
                            OriginalController._inventory.AddAt(Item, OriginPoint); // Return the item to its previous location
                        }
                        CurrentController._renderer.ClearSelection();
                    }
                    else
                    {
                        OriginalController._inventory.Drop(_draggedItem.Item); // Drop the item
                    }

                    // Destroy the image representing the item
                    GameObject.Destroy(_image.gameObject);
                }
            }
        }
}