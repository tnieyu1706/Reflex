using System;
using UnityEngine;
using UnityEngine.Pool;
using Reflex.Injectors;

namespace Reflex.Core
{
    [DefaultExecutionOrder(SceneContainerScopeExecutionOrder)]
    public class ContainerScope : MonoBehaviour
    {
        public const int SceneContainerScopeExecutionOrder = -1_000_000_000;

        [Tooltip("Drag and drop the parent Scene here. Leave empty to fallback to the Root/Project container.")]
        [SerializeField]
        private string _parentSceneName;

        public string ParentSceneName => _parentSceneName;

        private void Awake()
        {
            // Trigger scene injection. Runs before normal Awakes due to DefaultExecutionOrder.
            UnityInjector.OnSceneLoaded?.Invoke(gameObject.scene, this);
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using (ListPool<IInstaller>.Get(out var installers))
            {
                GetComponentsInChildren(installers);

                for (var i = 0; i < installers.Count; i++)
                {
                    installers[i].InstallBindings(containerBuilder);
                }
            }
        }

#if UNITY_EDITOR
        // Reference for Editor drag-and-drop support. Ignored in builds.
        [SerializeField] private UnityEditor.SceneAsset _parentSceneAsset;
#endif
    }
}