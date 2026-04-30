using System;
using UnityEngine;

namespace Reflex.Utilities
{
    /// <summary>
    /// A wrapper that provides the means to safely serialize Scene Asset References.
    /// Allows drag and drop in the editor, but survives builds.
    /// </summary>
    [Serializable]
    public class SceneReference : ISerializationCallbackReceiver
    {
#if UNITY_EDITOR
        // What we use in editor to select the scene
        [SerializeField] private UnityEditor.SceneAsset sceneAsset;
        private bool IsValidSceneAsset => sceneAsset != null;
#endif

        // This should only ever be set during serialization/deserialization!
        [SerializeField] private string scenePath = string.Empty;

        // Use this when you want to actually have the scene path
        public string ScenePath
        {
            get
            {
#if UNITY_EDITOR
                // In editor we always use the asset's path
                return GetScenePathFromAsset();
#else
                // At runtime we rely on the stored path value which we assume was serialized correctly at build time.
                return scenePath;
#endif
            }
            set
            {
                scenePath = value;
#if UNITY_EDITOR
                sceneAsset = GetSceneAssetFromPath();
#endif
            }
        }

        public string SceneName
        {
            get
            {
                var path = ScenePath;
                if (string.IsNullOrEmpty(path)) return string.Empty;

                int slash = path.LastIndexOf('/');
                string name = path.Substring(slash + 1);
                int dot = name.LastIndexOf('.');
                return dot > -1 ? name.Substring(0, dot) : name;
            }
        }

        public static implicit operator string(SceneReference sceneReference)
        {
            return sceneReference?.ScenePath;
        }

        // Called to prepare this data for serialization. Stubbed out when not in editor.
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            HandleBeforeSerialize();
#endif
        }

        // Called to set up data for deserialization. Stubbed out when not in editor.
        public void OnAfterDeserialize()
        {
#if UNITY_EDITOR
            // We sadly cannot touch assetdatabase during serialization, so defer by a bit.
            UnityEditor.EditorApplication.update += HandleAfterDeserialize;
#endif
        }

#if UNITY_EDITOR
        private UnityEditor.SceneAsset GetSceneAssetFromPath()
        {
            return string.IsNullOrEmpty(scenePath)
                ? null
                : UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.SceneAsset>(scenePath);
        }

        private string GetScenePathFromAsset()
        {
            return sceneAsset == null ? string.Empty : UnityEditor.AssetDatabase.GetAssetPath(sceneAsset);
        }

        private void HandleBeforeSerialize()
        {
            if (IsValidSceneAsset)
            {
                scenePath = GetScenePathFromAsset();
            }
            else
            {
                scenePath = string.Empty;
            }
        }

        private void HandleAfterDeserialize()
        {
            UnityEditor.EditorApplication.update -= HandleAfterDeserialize;
            if (IsValidSceneAsset)
            {
                return;
            }

            if (!string.IsNullOrEmpty(scenePath))
            {
                sceneAsset = GetSceneAssetFromPath();

                // If the scene was deleted or moved while Unity was closed, the path might be invalid.
                if (sceneAsset == null)
                {
                    scenePath = string.Empty;
                }
            }
        }
#endif
    }
}