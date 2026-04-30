using System;
using UnityEngine;
using UnityEngine.Pool;
using Reflex.Injectors;
using Reflex.Utilities;

namespace Reflex.Core
{
    [DefaultExecutionOrder(SceneContainerScopeExecutionOrder)]
    public class ContainerScope : MonoBehaviour
    {
        public const int SceneContainerScopeExecutionOrder = -1_000_000_000;

        [Tooltip("Drag and drop the parent Scene here. Leave empty to fallback to the Root/Project container.")]
        [SerializeField]
        private SceneReference _parentScene;

        // Trả về tên của Scene để UnityInjector xử lý, an toàn với null
        public string ParentSceneName => _parentScene != null ? _parentScene.SceneName : string.Empty;

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
    }
}