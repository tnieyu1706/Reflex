using UnityEditor;
using UnityEngine;
using Reflex.Utilities;

namespace Reflex.Editor.Utilities
{
    /// <summary>
    /// Custom PropertyDrawer for SceneReference, allowing drag and drop of SceneAsset in the Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        // Field names (must exactly match the variable names in the SceneReference class)
        private const string sceneAssetPropertyString = "sceneAsset";
        private const string scenePathPropertyString = "scenePath";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            // Find the SerializedProperties
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative(sceneAssetPropertyString);
            SerializedProperty scenePathProperty = property.FindPropertyRelative(scenePathPropertyString);

            // Draw the object drag and drop field (only display as SceneAsset)
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (sceneAssetProperty != null)
            {
                EditorGUI.BeginChangeCheck();

                // Display the Object picker for SceneAsset
                Object value = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue,
                    typeof(SceneAsset), false);

                if (EditorGUI.EndChangeCheck())
                {
                    sceneAssetProperty.objectReferenceValue = value;

                    // Update the hidden path (for runtime serialization)
                    if (sceneAssetProperty.objectReferenceValue != null)
                    {
                        scenePathProperty.stringValue =
                            AssetDatabase.GetAssetPath(sceneAssetProperty.objectReferenceValue);
                    }
                    else
                    {
                        scenePathProperty.stringValue = string.Empty;
                    }
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}