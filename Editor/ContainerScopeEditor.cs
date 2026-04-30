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

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hierarchy Configuration", EditorStyles.boldLabel);

            // Draw the Scene selection UI (Automatically calls SceneReferencePropertyDrawer)
            EditorGUILayout.PropertyField(_parentSceneProp,
                new GUIContent("Parent Scene", "Drop parent Scene here. Leave empty for Root Scope."));

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

                EditorGUILayout.HelpBox($"Runtime Parent Scene: '{sceneName}'", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Parent is empty. This container will attach to the Root (Project) Container.",
                    MessageType.None);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Installers", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Components inheriting IInstaller on this GameObject or its children will be auto-installed.",
                MessageType.Info);

            serializedObject.ApplyModifiedProperties();
        }
    }
}