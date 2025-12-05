using UnityEditor;
using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class BoxAttribute : PropertyAttribute {
        public string message;
        public MessageType messageType;

        public BoxAttribute(string message, MessageType type = MessageType.Info) {
            this.message = message;
            this.messageType = type;
        }
    }
    
    [CustomPropertyDrawer(typeof(BoxAttribute))]
    public class BoxAttributeDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            BoxAttribute box = (BoxAttribute)attribute;

            // Box
            Rect helpBoxRect = new Rect(position.x, position.y, position.width, 40f);
            EditorGUI.HelpBox(helpBoxRect, box.message, box.messageType);

            // Field
            Rect propertyRect = new Rect(position.x, position.y + 45f, position.width, EditorGUI.GetPropertyHeight(property, label, true));
            EditorGUI.PropertyField(propertyRect, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return 40f + EditorGUI.GetPropertyHeight(property, label, true) + 5f;
        }
    }
}