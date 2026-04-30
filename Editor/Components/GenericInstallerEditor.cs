using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Reflex.Editor
{
    public abstract class GenericInstallerEditor : UnityEditor.Editor
    {
        private SerializedProperty _bindingsProp;

        protected abstract string Title { get; }
        protected abstract string HelpText { get; }

        protected virtual void OnEnable()
        {
            _bindingsProp = serializedObject.FindProperty("_bindings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Title, EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(HelpText, MessageType.Info);

            for (int i = 0; i < _bindingsProp.arraySize; i++)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                var bindingProp = _bindingsProp.GetArrayElementAtIndex(i);
                var targetProp = bindingProp.FindPropertyRelative("Target");
                var contractsProp = bindingProp.FindPropertyRelative("Contracts");
                var isExpandedProp = bindingProp.FindPropertyRelative("_isExpanded");

                EditorGUILayout.BeginHorizontal();

                // Luôn hiển thị trường gán Target
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(targetProp, new GUIContent($"Target {i + 1}"));
                if (EditorGUI.EndChangeCheck())
                {
                    contractsProp.ClearArray();
                    isExpandedProp.boolValue = true; // Tự động mở foldout khi gán target mới
                }

                // Nút Remove
                if (GUILayout.Button("X", GUILayout.Width(24)))
                {
                    _bindingsProp.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                var targetObject = targetProp.objectReferenceValue;
                if (targetObject != null)
                {
                    EditorGUILayout.Space(2);

                    // Foldout dành riêng cho phần Contracts
                    isExpandedProp.boolValue =
                        EditorGUILayout.Foldout(isExpandedProp.boolValue, "Contracts Configuration", true);

                    if (isExpandedProp.boolValue)
                    {
                        DrawContractsSelection(targetObject, contractsProp);
                    }
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            if (GUILayout.Button("Add Binding", GUILayout.Height(24)))
            {
                _bindingsProp.arraySize++;
                var newElement = _bindingsProp.GetArrayElementAtIndex(_bindingsProp.arraySize - 1);
                newElement.FindPropertyRelative("Target").objectReferenceValue = null;
                newElement.FindPropertyRelative("Contracts").ClearArray();
                newElement.FindPropertyRelative("_isExpanded").boolValue = true;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawContractsSelection(UnityEngine.Object targetObject, SerializedProperty contractsProp)
        {
            var availableTypes = GetAvailableContracts(targetObject.GetType());
            var selectedTypeNames = GetSelectedTypeNames(contractsProp);

            if (availableTypes.Count == 0)
            {
                EditorGUILayout.HelpBox("No valid base class or interface found.", MessageType.Warning);
                return;
            }

            EditorGUI.indentLevel++;
            foreach (var type in availableTypes)
            {
                var typeName = type.AssemblyQualifiedName;
                bool isSelected = selectedTypeNames.Contains(typeName);

                // Chỉ hiển thị tên Class/Interface ngắn gọn
                string displayName = type.Name;

                bool newSelected = EditorGUILayout.ToggleLeft(displayName, isSelected);

                if (newSelected && !isSelected)
                {
                    AddTypeName(contractsProp, typeName);
                }
                else if (!newSelected && isSelected)
                {
                    RemoveTypeName(contractsProp, typeName);
                }
            }

            EditorGUI.indentLevel--;
        }

        private List<Type> GetAvailableContracts(Type type)
        {
            var types = new HashSet<Type>();
            types.Add(type);

            foreach (var iFace in type.GetInterfaces())
            {
                types.Add(iFace);
            }

            var currentBase = type.BaseType;
            // Bỏ qua các class gốc của Unity để list gọn gàng (dùng chung cho cả SC và Mono)
            while (currentBase != null &&
                   currentBase != typeof(MonoBehaviour) &&
                   currentBase != typeof(Behaviour) &&
                   currentBase != typeof(Component) &&
                   currentBase != typeof(ScriptableObject) &&
                   currentBase != typeof(UnityEngine.Object) &&
                   currentBase != typeof(object))
            {
                types.Add(currentBase);
                currentBase = currentBase.BaseType;
            }

            return types.OrderBy(t => t.Name).ToList();
        }

        private HashSet<string> GetSelectedTypeNames(SerializedProperty contractsProp)
        {
            var set = new HashSet<string>();
            for (int i = 0; i < contractsProp.arraySize; i++)
            {
                set.Add(contractsProp.GetArrayElementAtIndex(i).stringValue);
            }

            return set;
        }

        private void AddTypeName(SerializedProperty contractsProp, string typeName)
        {
            int index = contractsProp.arraySize;
            contractsProp.InsertArrayElementAtIndex(index);
            contractsProp.GetArrayElementAtIndex(index).stringValue = typeName;
        }

        private void RemoveTypeName(SerializedProperty contractsProp, string typeName)
        {
            for (int i = contractsProp.arraySize - 1; i >= 0; i--)
            {
                if (contractsProp.GetArrayElementAtIndex(i).stringValue == typeName)
                {
                    contractsProp.DeleteArrayElementAtIndex(i);
                }
            }
        }
    }
}