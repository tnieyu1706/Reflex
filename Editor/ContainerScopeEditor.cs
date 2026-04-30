using UnityEditor;
using UnityEngine;
using Reflex.Core;

namespace Reflex.Editor
{
    [CustomEditor(typeof(ContainerScope))]
    public class ContainerScopeEditor : UnityEditor.Editor
    {
        private SerializedProperty _parentSceneNameProp;
        private SerializedProperty _parentSceneAssetProp;

        private void OnEnable()
        {
            _parentSceneNameProp = serializedObject.FindProperty("_parentSceneName");
            _parentSceneAssetProp = serializedObject.FindProperty("_parentSceneAsset");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Hierarchy Configuration", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(_parentSceneAssetProp,
                new GUIContent("Parent Scene", "Drop parent Scene here. Leave empty for Root Scope."));

            if (EditorGUI.EndChangeCheck())
            {
                // Update string name when asset changes for runtime usage
                _parentSceneNameProp.stringValue = _parentSceneAssetProp.objectReferenceValue != null
                    ? _parentSceneAssetProp.objectReferenceValue.name
                    : string.Empty;
            }

            if (!string.IsNullOrEmpty(_parentSceneNameProp.stringValue))
            {
                EditorGUILayout.HelpBox($"Runtime Parent Scene: '{_parentSceneNameProp.stringValue}'",
                    MessageType.Info);
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