using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Components;
using UnityEditor;
using UnityEngine;

namespace Reflex.Editor
{
    /// <summary>
    /// Base custom editor for GenericInstaller. 
    /// Handles the drawing of bindings, target assignment, and contract selection.
    /// </summary>
    [CustomEditor(typeof(BaseGenericInstaller), true)]
    public class GenericInstallerEditor : UnityEditor.Editor
    {
        private SerializedProperty _bindingsProp;

        /// <summary>
        /// The title displayed at the top of the inspector.
        /// </summary>
        protected string Title => target.GetType().Name + " Bindings";

        protected virtual void OnEnable()
        {
            _bindingsProp = serializedObject.FindProperty("_bindings");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Title, EditorStyles.boldLabel);
            EditorGUILayout.Space(2);

            for (int i = 0; i < _bindingsProp.arraySize; i++)
            {
                // Using EditorStyles.helpBox provides a clean, unified border
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // FIX 1: Increment indent level inside the box so the foldout arrow doesn't bleed out of the left border
                EditorGUI.indentLevel++;

                var bindingProp = _bindingsProp.GetArrayElementAtIndex(i);
                var targetProp = bindingProp.FindPropertyRelative("Target");
                var contractsProp = bindingProp.FindPropertyRelative("Contracts");
                var isExpandedProp = bindingProp.FindPropertyRelative("_isExpanded");

                var targetObject = targetProp.objectReferenceValue;

                EditorGUILayout.BeginHorizontal();

                // FIX 2: Attach the label "Target N" directly to the Foldout instead of the PropertyField.
                // This prevents massive empty gaps and makes the layout highly compact.
                EditorGUI.BeginDisabledGroup(targetObject == null);
                isExpandedProp.boolValue = EditorGUILayout.Foldout(isExpandedProp.boolValue, $"Target {i + 1}", true);
                EditorGUI.EndDisabledGroup();

                // Target Assignment Field (Uses GUIContent.none to snap directly next to the foldout label)
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(targetProp, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    contractsProp.ClearArray();
                    isExpandedProp.boolValue = true; // Automatically open the foldout when a new target is assigned
                    targetObject = targetProp.objectReferenceValue; // Update local reference
                }

                // Remove Button
                if (GUILayout.Button("X", GUILayout.Width(22)))
                {
                    _bindingsProp.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    // Must decrement indent level before breaking out to avoid GUI layout stack errors
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                    break;
                }

                EditorGUILayout.EndHorizontal();

                // Contracts Selection
                if (targetObject != null && isExpandedProp.boolValue)
                {
                    EditorGUILayout.Space(2);
                    DrawContractsSelection(targetObject, contractsProp);
                    EditorGUILayout.Space(4);
                }

                // Decrement indent back for the next element
                EditorGUI.indentLevel--;
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

        /// <summary>
        /// Draws the list of available interfaces and base classes for the selected target.
        /// </summary>
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

                // Display only the short Class/Interface name for a cleaner UI
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

        /// <summary>
        /// Retrieves all implemented interfaces and valid base classes for a given type.
        /// Excludes native Unity classes to keep the list relevant.
        /// </summary>
        private List<Type> GetAvailableContracts(Type type)
        {
            var types = new HashSet<Type>();
            types.Add(type);

            foreach (var iFace in type.GetInterfaces())
            {
                types.Add(iFace);
            }

            var currentBase = type.BaseType;
            // Ignore Unity's base classes to keep the list clean
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