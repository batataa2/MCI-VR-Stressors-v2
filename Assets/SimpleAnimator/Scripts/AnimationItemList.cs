using System;
using UnityEditor;
using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    
    [Serializable]
    public class AnimationItemList {
        [DinamicDropdown("statusList.status", "name")]
        public int status;
        [DinamicDropdown("layerList.layers", "name")]
        public int layer;
        public AnimationData animation;
    }

    
    [CustomPropertyDrawer(typeof(AnimationItemList))]
    public class AnimationItemListDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            bool drawHeader = IsFirstElement(property);

            //
            SerializedProperty value1Prop = property.FindPropertyRelative("status");
            SerializedProperty value2Prop = property.FindPropertyRelative("layer");
            SerializedProperty value3Prop = property.FindPropertyRelative("animation");

            //
            float fieldWidth1 = position.width / 4f;
            float fieldWidth2 = position.width - fieldWidth1 * 2;
            float yOffset = drawHeader ? EditorGUIUtility.singleLineHeight : 0f;
            float totalHeight = GetPropertyHeight(property, label);
            float fieldHeight = totalHeight - yOffset;

            // Header rects
            if (drawHeader) {
                EditorGUI.LabelField(new Rect(position.x, position.y, fieldWidth1, yOffset), "Status", EditorStyles.boldLabel);
                EditorGUI.LabelField(new Rect(position.x + fieldWidth1, position.y, fieldWidth1, yOffset), "Layer", EditorStyles.boldLabel);
                EditorGUI.LabelField(new Rect(position.x + fieldWidth1 * 2, position.y, fieldWidth2, yOffset), "Animation", EditorStyles.boldLabel);
            }

            // Field rects
            Rect value1Rect = new Rect(position.x, position.y + yOffset, fieldWidth1, fieldHeight);
            Rect value2Rect = new Rect(position.x + fieldWidth1, position.y + yOffset, fieldWidth1, fieldHeight);
            Rect value3Rect = new Rect(position.x + fieldWidth1 * 2, position.y + yOffset, fieldWidth2, fieldHeight);

            //Field
            EditorGUI.PropertyField(value1Rect, value1Prop, GUIContent.none);
            EditorGUI.PropertyField(value2Rect, value2Prop, GUIContent.none);
            EditorGUI.PropertyField(value3Rect, value3Prop, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            bool drawHeader = IsFirstElement(property);
            return drawHeader ? EditorGUIUtility.singleLineHeight * 2 : EditorGUIUtility.singleLineHeight;
        }

        private bool IsFirstElement(SerializedProperty property) {
            if (property.propertyPath.EndsWith("Array.data[0]")) return true;

            var path = property.propertyPath;
            var indexStart = path.LastIndexOf("[") + 1;
            var indexEnd = path.LastIndexOf("]");
            if (indexStart > 0 && indexEnd > indexStart) {
                string indexStr = path.Substring(indexStart, indexEnd - indexStart);
                if (int.TryParse(indexStr, out int index)) {
                    return index == 0;
                }
            }
            return false;
        }
    }
}