#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace AionGames.SimpleAnimatorPackage {
    [InitializeOnLoad]
    public static class ScriptableObjectSelectionWatcher{
        private static Object lastSelectedObject = null;

        static ScriptableObjectSelectionWatcher(){
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static void OnSelectionChanged(){
            if (lastSelectedObject != null){
                CallApplyFinalChanges(lastSelectedObject);
            }
            
            lastSelectedObject = Selection.activeObject;
        }

        private static void CallApplyFinalChanges(Object obj){
            if (obj is ScriptableObject scriptableObject){
                MethodInfo method = scriptableObject.GetType().GetMethod("ApplyFinalChanges", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                
                if (method != null){
                    method.Invoke(scriptableObject, null);
                }
            }
        }
    }
}
#endif