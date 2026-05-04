using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Reflex.Editor
{
    [CustomPropertyDrawer(typeof(Components.GenericBinding<>), true)]
    public class GenericBindingDrawer : PropertyDrawer
    {
        private Dictionary<string, bool> _foldoutStates = new();
        private Dictionary<string, List<Type>> _cachedTypes = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isExpanded = GetFoldout(property);

            if (!isExpanded)
                return EditorGUIUtility.singleLineHeight;

            var targetProp = property.FindPropertyRelative("Target");
            int count = GetValidTypes(property, targetProp).Count;

            return EditorGUIUtility.singleLineHeight * (1 + count);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var targetProp = property.FindPropertyRelative("Target");
            var contractsProp = property.FindPropertyRelative("Contracts");

            float lineHeight = EditorGUIUtility.singleLineHeight;

            // ===== Foldout + Target cùng dòng =====
            Rect foldRect = new Rect(position.x, position.y, 15, lineHeight);
            Rect targetRect = new Rect(position.x + 15, position.y, position.width - 15, lineHeight);

            bool expanded = GetFoldout(property);
            expanded = EditorGUI.Foldout(foldRect, expanded, GUIContent.none);
            SetFoldout(property, expanded);

            EditorGUI.PropertyField(targetRect, targetProp, GUIContent.none);

            if (!expanded)
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

            var validTypes = GetValidTypes(property, targetProp);

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
                }

                y += lineHeight;
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        // =========================
        // Foldout State
        // =========================

        private bool GetFoldout(SerializedProperty property)
        {
            if (_foldoutStates.TryGetValue(property.propertyPath, out var state))
                return state;

            return false;
        }

        private void SetFoldout(SerializedProperty property, bool value)
        {
            _foldoutStates[property.propertyPath] = value;
        }

        // =========================
        // Helpers
        // =========================

        private List<Type> GetValidTypes(SerializedProperty property, SerializedProperty targetProp)
        {
            var targetObj = targetProp.objectReferenceValue;
            if (targetObj == null)
                return new List<Type>();

            var targetType = targetObj.GetType();
            string key = targetType.FullName;

            if (_cachedTypes.TryGetValue(key, out var cached))
                return cached;

            var result = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t =>
                    t.IsAssignableFrom(targetType) &&
                    t != targetType &&
                    !typeof(UnityEngine.Object).IsAssignableFrom(t)
                )
                .OrderBy(t => t.IsInterface ? 0 : 1) // interface lên trước
                .ThenBy(t => t.Name)
                .ToList();

            _cachedTypes[key] = result;

            return result;
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