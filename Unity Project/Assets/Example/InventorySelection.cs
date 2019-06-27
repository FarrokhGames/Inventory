using System;
using UnityEngine;
using UnityEngine.UI;

namespace FarrokhGames.Inventory.Examples
{
    public class InventorySelection : MonoBehaviour
    {
        Text _text;

        void Start()
        {
            _text = GetComponentInChildren<Text>();
            _text.text = string.Empty;

            var allControllers = GameObject.FindObjectsOfType<InventoryController>();

            UnityEngine.Debug.Log("Found " + allControllers.Length + " controllers");
            foreach (var controller in allControllers)
            {
                controller.OnItemHovered += HandleItemHover;
            }
        }

        private void HandleItemHover(IInventoryItem item)
        {
            _text.text = item != null ? item.Sprite.name : string.Empty;
        }
    }
}