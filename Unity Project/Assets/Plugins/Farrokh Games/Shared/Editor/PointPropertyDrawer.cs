using UnityEditor;
using UnityEngine;

namespace FarrokhGames.Shared
{
    /// <summary>
    /// Custom Property Drawer for Point
    /// </summary>
    [CustomPropertyDrawer(typeof(Point))]
    public class PointPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Find properties
            var x = property.FindPropertyRelative("x");
            var y = property.FindPropertyRelative("y");

            // Begin property
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Fix intent
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var halfWidth = position.width / 2;
            var xRect = new Rect(position.x, position.y, halfWidth - 4, position.height);
            var yRect = new Rect(position.x + halfWidth, position.y, halfWidth, position.height);

            // X & Y
            EditorGUIUtility.labelWidth = 12;
            EditorGUI.PropertyField(xRect, x, new GUIContent("x"));
            EditorGUI.PropertyField(yRect, y, new GUIContent("y"));

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            // End property
            EditorGUI.EndProperty();
        }
    }
}