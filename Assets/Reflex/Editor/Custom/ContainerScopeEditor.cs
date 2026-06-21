using UnityEditor;
using UnityEngine;
using Reflex.Core;

namespace Reflex.Editor
{
    [CustomEditor(typeof(ContainerScope))]
    public class ContainerScopeEditor : UnityEditor.Editor
    {
        private SerializedProperty _parentSceneProp;

        private void OnEnable()
        {
            // Point to the new variable in ContainerScope
            _parentSceneProp = serializedObject.FindProperty("_parentScene");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.BeginHorizontal();
            // Draw the Scene selection UI (Automatically calls SceneReferencePropertyDrawer)
            EditorGUILayout.PropertyField(_parentSceneProp,
                new GUIContent("Parent Scene", "Drop parent Scene here. Leave empty for Root Scope."));

            // Thêm nút Clear để reset về None/Root Scope
            if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(45)))
            {
                var pathProp = _parentSceneProp.FindPropertyRelative("scenePath");
                if (pathProp != null) pathProp.stringValue = string.Empty;

                // Cố gắng dọn dẹp biến chứa Object/Asset bên trong struct (nếu có)
                var assetProp = _parentSceneProp.FindPropertyRelative("sceneAsset") 
                                ?? _parentSceneProp.FindPropertyRelative("asset")
                                ?? _parentSceneProp.FindPropertyRelative("_sceneAsset");
                if (assetProp != null) assetProp.objectReferenceValue = null;

                // Xoá focus để drawer cập nhật lại UI ngay lập tức
                GUI.FocusControl(null); 
            }
            EditorGUILayout.EndHorizontal();

            // Extract the path to display the Scene name in the HelpBox
            var scenePathProp = _parentSceneProp.FindPropertyRelative("scenePath");
            string currentPath = scenePathProp != null ? scenePathProp.stringValue : string.Empty;

            if (!string.IsNullOrEmpty(currentPath))
            {
                // Parse the Scene name from the path
                int slash = currentPath.LastIndexOf('/');
                string nameWithExt = currentPath.Substring(slash + 1);
                int dot = nameWithExt.LastIndexOf('.');
                string sceneName = dot > -1 ? nameWithExt.Substring(0, dot) : nameWithExt;

                EditorGUILayout.HelpBox($"Parent => [{sceneName}]", MessageType.None);
            }
            else
            {
                EditorGUILayout.HelpBox("Empty => [Root].",
                    MessageType.None);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}