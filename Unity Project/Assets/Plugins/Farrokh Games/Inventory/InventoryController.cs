using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FarrokhGames.Inventory
{
    public interface IInventoryController
    {
        Action<IInventoryItem> onItemHovered { get; set; }
        Action<IInventoryItem> onItemPickedUp { get; set; }
        Action<IInventoryItem> onItemAdded { get; set; }
        Action<IInventoryItem> onItemSwapped { get; set; }
        Action<IInventoryItem> onItemReturned { get; set; }
        Action<IInventoryItem> onItemDropped { get; set; }
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
            private static DraggedItem _draggedItem;

            /// <inheritdoc />
            public Action<IInventoryItem> onItemHovered { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> onItemPickedUp { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> onItemAdded { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> onItemSwapped { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> onItemReturned { get; set; }

            /// <inheritdoc />
            public Action<IInventoryItem> onItemDropped { get; set; }

            private Canvas _canvas;
            internal InventoryRenderer inventoryRenderer;
            internal IInventoryManager inventory => inventoryRenderer.inventory;

            private IInventoryItem _itemToDrag;
            private PointerEventData _currentEventData;
            private IInventoryItem _lastHoveredItem;

            /*
             * Setup
             */
            void Awake()
            {
                inventoryRenderer = GetComponent<InventoryRenderer>();
                if (inventoryRenderer == null) { throw new NullReferenceException("Could not find a renderer. This is not allowed!"); }

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
                if (_draggedItem != null) return;
                // Get which item to drag (item will be null of none were found)
                var grid = ScreenToGrid(eventData.position);
                _itemToDrag = inventory.GetAtPoint(grid);
            }

            /*
             * Dragging started (IBeginDragHandler)
             */
            public void OnBeginDrag(PointerEventData eventData)
            {
                inventoryRenderer.ClearSelection();

                if (_itemToDrag == null || _draggedItem != null) return;
                
                var localPosition = ScreenToLocalPositionInRenderer(eventData.position);
                var itemOffest = inventoryRenderer.GetItemOffset(_itemToDrag);
                var offset = itemOffest - localPosition;

                // Create a dragged item 
                _draggedItem = new DraggedItem(
                    _canvas.transform as RectTransform,
                    this,
                    _itemToDrag.position,
                    _itemToDrag,
                    offset
                );

                // Remove the item from inventory
                inventory.TryRemove(_itemToDrag);

                onItemPickedUp?.Invoke(_itemToDrag);
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
                if (_draggedItem == null) return;
                
                var mode = _draggedItem.Drop(eventData.position);

                switch (mode)
                {
                    case DraggedItem.DropMode.Added:
                        onItemAdded?.Invoke(_itemToDrag);
                        break;
                    case DraggedItem.DropMode.Swapped:
                        onItemSwapped?.Invoke(_itemToDrag);
                        break;
                    case DraggedItem.DropMode.Returned:
                        onItemReturned?.Invoke(_itemToDrag);
                        break;
                    case DraggedItem.DropMode.Dropped:
                        onItemDropped?.Invoke(_itemToDrag);
                        ClearHoveredItem();
                        break;
                }

                _draggedItem = null;
            }

            /*
             * Pointer left the inventory (IPointerExitHandler)
             */
            public void OnPointerExit(PointerEventData eventData)
            {
                if (_draggedItem != null)
                {
                    // Clear the item as it leaves its current controller
                    _draggedItem.currentController = null;
                    inventoryRenderer.ClearSelection();
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
                    _draggedItem.currentController = this;
                }
                _currentEventData = eventData;
            }

            /*
             * Update loop
             */
            void Update()
            {
                if (_currentEventData == null) return;
                
                if (_draggedItem == null)
                {
                    // Detect hover
                    var grid = ScreenToGrid(_currentEventData.position);
                    var item = inventory.GetAtPoint(grid);
                    if (item == _lastHoveredItem) return;
                    onItemHovered?.Invoke(item);
                    _lastHoveredItem = item;
                }
                else
                {
                    // Update position while dragging
                    _draggedItem.position = _currentEventData.position;
                }
            }

            /* 
             * 
             */
            private void ClearHoveredItem()
            {
                if (_lastHoveredItem != null)
                {
                    onItemHovered?.Invoke(null);
                }
                _lastHoveredItem = null;
            }

            /*
             * Get a point on the grid from a given screen point
             */
            internal Vector2Int ScreenToGrid(Vector2 screenPoint)
            {
                var pos = ScreenToLocalPositionInRenderer(screenPoint);
                var sizeDelta = inventoryRenderer.rectTransform.sizeDelta;
                pos.x += sizeDelta.x / 2;
                pos.y += sizeDelta.y / 2;
                return new Vector2Int(Mathf.FloorToInt(pos.x / inventoryRenderer.cellSize.x), Mathf.FloorToInt(pos.y / inventoryRenderer.cellSize.y));
            }

            /*
             * Returns the offset between dragged item and the grid 
             */
            internal Vector2 GetDraggedItemOffset(IInventoryItem item)
            {
                var gx = -((item.width * inventoryRenderer.cellSize.x) / 2f) + (inventoryRenderer.cellSize.x / 2);
                var gy = -((item.height * inventoryRenderer.cellSize.y) / 2f) + (inventoryRenderer.cellSize.y / 2);
                return new Vector2(gx, gy);
            }

            private Vector2 ScreenToLocalPositionInRenderer(Vector2 screenPosition)
            {
                Vector2 localPosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    inventoryRenderer.rectTransform,
                    screenPosition,
                    _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
                    out localPosition
                );
                return localPosition;
            }
        }
}