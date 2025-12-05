using UnityEditor;
using UnityEngine;

namespace AionGames.SimpleAnimatorPackage {
    public class ConditionalHideAttribute  : PropertyAttribute{
        public string ConditionField;
        public bool InvertCondition;

        public ConditionalHideAttribute(string conditionField, bool invertCondition = false){
            ConditionField = conditionField;
            InvertCondition = invertCondition;
        }
    }
    
    [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
    public class ConditionalHideDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            
            string path = property.propertyPath;
            string basePath = path.Substring(0, path.LastIndexOf('.'));
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(basePath + "." + condHAtt.ConditionField);
            
            bool conditionMet = conditionProperty != null && conditionProperty.boolValue;
            if (condHAtt.InvertCondition) conditionMet = !conditionMet;
            
            if (conditionMet) {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            ConditionalHideAttribute condHAtt = (ConditionalHideAttribute)attribute;
            
            string path = property.propertyPath; 
            string basePath = path.Substring(0, path.LastIndexOf('.'));
            SerializedProperty conditionProperty = property.serializedObject.FindProperty(basePath + "." + condHAtt.ConditionField);
            
            bool conditionMet = conditionProperty != null && conditionProperty.boolValue;
            if (condHAtt.InvertCondition) conditionMet = !conditionMet;

            return conditionMet ? EditorGUI.GetPropertyHeight(property, label, true) : 0;
        }
    }
}