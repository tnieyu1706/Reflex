using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Extensions;
using Reflex.Templates;
using UnityEditor;
using UnityEngine;

namespace Reflex.Editor
{
    [CustomPropertyDrawer(typeof(GenericBinding<>), true)]
    public class GenericBindingDrawer : PropertyDrawer
    {
        // Cache reflection results to prevent lag during Inspector rendering
        private static readonly Dictionary<string, List<Type>> _cachedTypes = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            var targetProp = property.FindPropertyRelative("Target");
            
            if (targetProp.objectReferenceValue == null)
                return EditorGUIUtility.singleLineHeight * 2; // Target field + "Target is null" label

            int count = GetValidTypes(targetProp).Count;
            return EditorGUIUtility.singleLineHeight * (1 + count);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("Target");
            var contractsProp = property.FindPropertyRelative("Contracts");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect foldRect = new Rect(position.x, position.y, 15, lineHeight);
            Rect targetRect = new Rect(position.x + 15, position.y, position.width - 15, lineHeight);

            property.isExpanded = EditorGUI.Foldout(foldRect, property.isExpanded, GUIContent.none);

            // Detect target changes to clear outdated contracts
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);
            if (EditorGUI.EndChangeCheck())
            {
                contractsProp.ClearArray();
            }

            if (!property.isExpanded)
            {
                EditorGUI.EndProperty();
                return;
            }

            EditorGUI.indentLevel++;
            float y = position.y + lineHeight;

            var targetObj = targetProp.objectReferenceValue;
            if (targetObj == null)
            {
                EditorGUI.LabelField(new Rect(position.x, y, position.width, lineHeight), "Target is null");
                EditorGUI.indentLevel--;
                EditorGUI.EndProperty();
                return;
            }

            var validTypes = GetValidTypes(targetProp);

            HashSet<string> selected = new();
            for (int i = 0; i < contractsProp.arraySize; i++)
            {
                selected.Add(contractsProp.GetArrayElementAtIndex(i).stringValue);
            }

            foreach (var type in validTypes)
            {
                Rect rect = new Rect(position.x, y, position.width, lineHeight);

                bool isChecked = selected.Contains(type.AssemblyQualifiedName);
                bool newChecked = EditorGUI.ToggleLeft(rect, type.Name, isChecked);

                if (newChecked != isChecked)
                {
                    if (newChecked)
                        AddContract(contractsProp, type);
                    else
                        RemoveContract(contractsProp, type);
                        
                    // Apply changes immediately for UI refresh and Undo support
                    property.serializedObject.ApplyModifiedProperties();
                }

                y += lineHeight;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        // =========================
        // Helpers
        // =========================

        static readonly HashSet<Type> IgnoreBaseTypes = new()
        {
            typeof(System.Object),
            typeof(UnityEngine.Object),
            typeof(Component),
            typeof(Behaviour),
            typeof(MonoBehaviour),
            typeof(ScriptableObject),
        };

        private List<Type> GetValidTypes(SerializedProperty targetProp)
        {
            var targetObj = targetProp.objectReferenceValue;
            if (targetObj == null)
                return new List<Type>();

            var targetType = targetObj.GetType();
            string key = targetType.FullName;

            if (_cachedTypes.TryGetValue(key, out var cached))
                return cached;

            var parentTypes = targetType.GetAllParentTypesWithSelf();
            var ignoreBaseTypes = IgnoreBaseTypes;

            var resultTypes = parentTypes
                .Where(t => !ignoreBaseTypes.Contains(t))
                .OrderBy(t => t.IsInterface ? 0 : 1)
                .ThenBy(t => t.Name)
                .ToList();

            _cachedTypes[key] = resultTypes;

            return resultTypes;
        }

        private void AddContract(SerializedProperty contractsProp, Type type)
        {
            int index = contractsProp.arraySize;
            contractsProp.InsertArrayElementAtIndex(index);
            contractsProp.GetArrayElementAtIndex(index).stringValue = type.AssemblyQualifiedName;
        }

        private void RemoveContract(SerializedProperty contractsProp, Type type)
        {
            for (int i = 0; i < contractsProp.arraySize; i++)
            {
                if (contractsProp.GetArrayElementAtIndex(i).stringValue == type.AssemblyQualifiedName)
                {
                    contractsProp.DeleteArrayElementAtIndex(i);
                    return;
                }
            }
        }
    }
}