using UnityEditor;
using UnityEngine;

namespace FarrokhGames.Inventory
{
    /// <summary>
    /// Custom Property Drawer for InventoryShape
    /// </summary>
    [CustomPropertyDrawer(typeof(InventoryShape))]
    public class InventoryShapePropertyDrawer : PropertyDrawer
    {
        const int GridSize = 16; // The size between the boold-fields that make up the shape matrix

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Find properties
            var pWidth = property.FindPropertyRelative("_width");
            var pHeight = property.FindPropertyRelative("_height");
            var pShape = property.FindPropertyRelative("_shape");

            // Clamp height & width
            if (pWidth.intValue <= 0) { pWidth.intValue = 1; }
            if (pHeight.intValue <= 0) { pHeight.intValue = 1; }

            // Begin property
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Fix intent
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var halfWidth = position.width / 2;
            var widthRect = new Rect(position.x, position.y, halfWidth, GridSize);
            var heightRect = new Rect(position.x + halfWidth, position.y, halfWidth, GridSize);

            // Width & Height
            EditorGUIUtility.labelWidth = 40;
            EditorGUI.PropertyField(widthRect, pWidth, new GUIContent("width"));
            EditorGUI.PropertyField(heightRect, pHeight, new GUIContent("height"));

            // Draw grid
            var width = pWidth.intValue;
            var height = pHeight.intValue;
            pShape.arraySize = width * height;
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var index = x + width * y;
                    var rect = new Rect(position.x + (x * GridSize), position.y + GridSize + (y * GridSize), GridSize, GridSize);
                    EditorGUI.PropertyField(rect, pShape.GetArrayElementAtIndex(index), GUIContent.none);
                }
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            // End property
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label);
            height += property.FindPropertyRelative("_height").intValue * GridSize;
            return height;
        }
    }
}