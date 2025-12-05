using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class DinamicDropdownAttribute: PropertyAttribute {
        public string listProperty;
        public string displayProperty;
        
        public DinamicDropdownAttribute(string listProperty, string displayProperty) {
            this.listProperty = listProperty;
            this.displayProperty = displayProperty;
        }
    }
    [CustomPropertyDrawer(typeof(DinamicDropdownAttribute))]
    public class DinamicDropdownDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            DinamicDropdownAttribute dropdownAttribute = (DinamicDropdownAttribute)attribute;
            SerializedObject obj = property.serializedObject;
            
            object listObject = GetNestedPropertyValue(obj.targetObject, dropdownAttribute.listProperty);
            if (listObject is IList<object> list) {
                List<string> options = new List<string>();
                foreach (var item in list) {
                    string displayValue = GetFieldValue(item, dropdownAttribute.displayProperty);
                    options.Add(string.IsNullOrEmpty(displayValue) ? "Unnamed" : displayValue);
                }

                property.intValue = EditorGUI.Popup(position, label.text, property.intValue, options.ToArray());
            } else {
                EditorGUI.LabelField(position, label.text, "Dropdown Error: List not found");
            }
        }
        
        private object GetNestedPropertyValue(object obj, string propertyPath) {
            if (obj == null) return null;
            string[] properties = propertyPath.Split('.');
            foreach (string property in properties) {
                if (obj == null) return null;
                FieldInfo field = obj.GetType().GetField(property, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                obj = field?.GetValue(obj);
            }
            return obj;
        }
        
        private string GetFieldValue(object obj, string fieldName) {
            if (obj == null) return null;
            FieldInfo field = obj.GetType().GetField(fieldName);
            return field != null ? field.GetValue(obj)?.ToString() : null;
        }
    }
}