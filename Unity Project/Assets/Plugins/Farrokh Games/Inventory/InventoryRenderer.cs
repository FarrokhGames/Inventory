using System;
using System.Collections.Generic;
using FarrokhGames.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Renders a given inventory
    /// /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class InventoryRenderer : MonoBehaviour
    {
        [SerializeField, Tooltip("The size of the cells building up the inventory")]
        private Vector2Int _cellSize = new Vector2Int(32, 32);

        [SerializeField, Tooltip("The sprite to use for empty cells")]
        private Sprite _cellSpriteEmpty;

        [SerializeField, Tooltip("The sprite to use for selected cells")]
        private Sprite _cellSpriteSelected;

        [SerializeField, Tooltip("The sprite to use for blocked cells")]
        private Sprite _cellSpriteBlocked;

        internal InventoryManager _inventory = null;
        private bool _haveListeners = false;
        private Pool<Image> _imagePool;
        private Image[] _grids = null;
        private Dictionary<IInventoryItem, Image> _items = new Dictionary<IInventoryItem, Image>();

        /*
         * Setup
         */
        void Awake()
        {
            RectTransform = GetComponent<RectTransform>();

            // Create the image container
            var imageContainer = new GameObject("Image Pool").AddComponent<RectTransform>();
            imageContainer.transform.SetParent(transform);
            imageContainer.transform.localPosition = Vector3.zero;
            imageContainer.transform.localScale = Vector3.one;

            // Create pool of images
            _imagePool = new Pool<Image>(
                delegate
                {
                    var image = new GameObject("Image").AddComponent<Image>();
                    image.transform.SetParent(imageContainer);
                    image.transform.localScale = Vector3.one;
                    return image;
                },
                0,
                true);
        }

        /// <summary>
        /// Set what inventory to use when rendering
        /// </summary>
        /// <param name="inventory">Inventory to use</param>
        public void SetInventory(InventoryManager inventory)
        {
            if (inventory == null) { throw new ArgumentNullException("inventory"); }
            OnDisable();
            _inventory = inventory;
            OnEnable();
        }

        /// <summary>
        /// Returns the RectTransform for this renderer
        /// </summary>
        public RectTransform RectTransform { get; private set; }

        /// <summary>
        /// Returns the size of this inventory's cells
        /// </summary>
        public Vector2 CellSize { get { return _cellSize; } }

        /* 
        Invoked when the Inventory Renderer is enabled
        */
        void OnEnable()
        {
            if (_inventory != null && !_haveListeners)
            {
                if (_cellSpriteEmpty == null) { throw new ArgumentNullException("Sprite for empty cell is null."); }
                if (_cellSpriteSelected == null) { throw new ArgumentNullException("Sprite for selected cells is null."); }
                if (_cellSpriteBlocked == null) { throw new ArgumentNullException("Sprite for blocked cells is null."); }

                _inventory.OnCleared += ReRenderAllItems;
                _inventory.OnItemAdded += HandleItemAdded;
                _inventory.OnItemRemoved += HandleItemRemoved;
                _inventory.OnItemDropped += HandleItemRemoved;
                _inventory.OnResized += HandleResized;
                _haveListeners = true;

                // Render Inventory
                ReRenderGrid();
                ReRenderAllItems();
            }
        }

        /* 
        Invoked when the Inventory Renderer is disabled
        */
        void OnDisable()
        {
            if (_inventory != null && _haveListeners)
            {
                _inventory.OnCleared -= ReRenderAllItems;
                _inventory.OnItemAdded -= HandleItemAdded;
                _inventory.OnItemRemoved -= HandleItemRemoved;
                _inventory.OnItemDropped -= HandleItemRemoved;
                _inventory.OnResized -= HandleResized;
                _haveListeners = false;
            }
        }

        /*
        Clears and renders the grid. This must be done whenever the size of the inventory changes
        */
        private void ReRenderGrid()
        {
            // Clear the grid
            if (_grids != null)
            {
                for (var i = 0; i < _grids.Length; i++)
                {
                    _grids[i].gameObject.SetActive(false);
                    RecycleImage(_grids[i]);
                    _grids[i].transform.SetSiblingIndex(i);
                }
            }
            _grids = null;

            // Render new grid
            var containerSize = new Vector2(CellSize.x * _inventory.Width, CellSize.y * _inventory.Height);
            var topLeft = new Vector3(-containerSize.x / 2, -containerSize.y / 2, 0); // Calculate topleft corner
            var halfCellSize = new Vector3(CellSize.x / 2, CellSize.y / 2, 0); // Calulcate cells half-size

            // Spawn grid images
            _grids = new Image[_inventory.Width * _inventory.Height];
            var c = 0;
            for (int y = 0; y < _inventory.Height; y++)
            {
                for (int x = 0; x < _inventory.Width; x++)
                {
                    var grid = CreateImage(_cellSpriteEmpty, true);
                    grid.gameObject.name = "Grid " + c;
                    grid.rectTransform.SetAsFirstSibling();
                    grid.type = Image.Type.Sliced;
                    grid.rectTransform.localPosition = topLeft + new Vector3(CellSize.x * ((_inventory.Width - 1) - x), CellSize.y * y, 0) + halfCellSize;
                    grid.rectTransform.sizeDelta = CellSize;
                    _grids[c] = grid;
                    c++;
                }
            }

            // Set the size of the main RectTransform
            // This is useful as it allowes custom graphical elements
            // suchs as a border to mimic the size of the inventory.
            RectTransform.sizeDelta = containerSize;
        }

        /*
        Clears and renders all items
        */
        private void ReRenderAllItems()
        {
            // Clear all items
            foreach (var image in _items.Values)
            {
                image.gameObject.SetActive(false);
                RecycleImage(image);
            }
            _items.Clear();

            // Add all items
            foreach (var item in _inventory.AllItems)
            {
                HandleItemAdded(item);
            }
        }

        /*
        Handler for when Inventory.OnItemAdded is invoked
        */
        private void HandleItemAdded(IInventoryItem item)
        {
            var img = CreateImage(item.Sprite, false);
            img.gameObject.name = item.Name;
            img.rectTransform.localPosition = GetItemOffset(item);
            _items.Add(item, img);
        }

        /*
        Handler for when Inventory.OnItemRemoved is invoked
        */
        private void HandleItemRemoved(IInventoryItem item)
        {
            if (_items.ContainsKey(item))
            {
                var image = _items[item];
                image.gameObject.SetActive(false);
                RecycleImage(image);
                _items.Remove(item);
            }
        }

        /*
        Handler for when Inventory.OnResized is invoked
        */
        private void HandleResized()
        {
            ReRenderGrid();
            ReRenderAllItems();
        }

        /*
         * Create an image with given sprite and settings
         */
        private Image CreateImage(Sprite sprite, bool raycastTarget)
        {
            var img = _imagePool.Take();
            img.gameObject.SetActive(true);
            img.sprite = sprite;
            img.rectTransform.sizeDelta = new Vector2(img.sprite.rect.width, img.sprite.rect.height);
            img.transform.SetAsLastSibling();
            img.type = Image.Type.Simple;
            img.raycastTarget = raycastTarget;
            return img;
        }

        /*
         * Recycles given image 
         */
        private void RecycleImage(Image image)
        {
            image.gameObject.name = "Image";
            image.gameObject.SetActive(false);
            _imagePool.Recycle(image);
        }

        /// <summary>
        /// Selects a given item in the inventory
        /// </summary>
        /// <param name="item">Item to select</param>
        /// <param name="blocked">Should the selection be rendered as blocked</param>
        /// <param name="color">The color of the selection</param>
        public void SelectItem(IInventoryItem item, bool blocked, Color color)
        {
            if (item == null) { return; }
            Select(item.Shape.Points, blocked, color);
        }

        /// <summary>
        /// Selects a single point in the inventory
        /// </summary>
        /// <param name="point">Point to select</param>
        /// <param name="blocked">Should the selection be rendered as blocked</param>
        /// <param name="color">The color of the selection</param>
        private void Select(Vector2Int point, bool blocked, Color color)
        {
            Select(new Vector2Int[] { point }, blocked, color);
        }

        /// <summary>
        /// Selects given points in this inventory
        /// </summary>
        /// <param name="points">Points to select</param>
        /// <param name="blocked">Should the selection be rendered as blocked</param>
        /// <param name="color">The color of the selection</param>
        private void Select(Vector2Int[] points, bool blocked, Color color)
        {
            ClearSelection();
            for (var i = 0; i < points.Length; i++)
            {
                var p = points[i];
                if (p.x >= 0 && p.x < _inventory.Width && p.y >= 0 && p.y < _inventory.Height)
                {
                    var index = p.y * _inventory.Width + ((_inventory.Width - 1) - p.x);
                    _grids[index].sprite = blocked ? _cellSpriteBlocked : _cellSpriteSelected;
                    _grids[index].color = color;
                }
            }
        }

        /// <summary>
        /// Clears all selections made in this inventory
        /// </summary>
        public void ClearSelection()
        {
            for (var i = 0; i < _grids.Length; i++)
            {
                _grids[i].sprite = _cellSpriteEmpty;
                _grids[i].color = Color.white;
            }
        }

        /*
        Returns the appropriate offset of an item to make it fit nicely in the grid
        */
        internal Vector2 GetItemOffset(IInventoryItem item)
        {
            var x = (-(_inventory.Width * 0.5f) + item.Shape.Position.x + ((float) item.Shape.Width * 0.5f)) * CellSize.x;
            var y = (-(_inventory.Height * 0.5f) + item.Shape.Position.y + ((float) item.Shape.Height * 0.5f)) * CellSize.y;
            return new Vector2(x, y);
        }
    }
}