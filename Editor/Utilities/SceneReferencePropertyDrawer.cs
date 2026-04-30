using UnityEditor;
using UnityEngine;
using Reflex.Utilities;

namespace Reflex.Editor.Utilities
{
    /// <summary>
    /// Custom PropertyDrawer cho SceneReference, cho phép kéo thả SceneAsset trên Inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawer : PropertyDrawer
    {
        // Tên các field (phải khớp chính xác với tên biến trong class SceneReference)
        private const string sceneAssetPropertyString = "sceneAsset";
        private const string scenePathPropertyString = "scenePath";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);

            // Tìm các SerializedProperties
            SerializedProperty sceneAssetProperty = property.FindPropertyRelative(sceneAssetPropertyString);
            SerializedProperty scenePathProperty = property.FindPropertyRelative(scenePathPropertyString);

            // Vẽ trường kéo thả đối tượng (chỉ hiển thị dưới dạng SceneAsset)
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (sceneAssetProperty != null)
            {
                EditorGUI.BeginChangeCheck();

                // Hiển thị ô chọn Object kiểu SceneAsset
                Object value = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue,
                    typeof(SceneAsset), false);

                if (EditorGUI.EndChangeCheck())
                {
                    sceneAssetProperty.objectReferenceValue = value;

                    // Cập nhật đường dẫn ẩn (để serialize cho runtime)
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