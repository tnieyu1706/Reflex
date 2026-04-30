using Reflex.Injectors;
using Reflex.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    /// <summary>
    /// Responsible for creating and managing the Dependency Injection Container for a Unity Scene.
    /// It collects all components implementing <see cref="IInstaller"/> in its hierarchy to build the scene's isolated environment.
    /// </summary>
    [DefaultExecutionOrder(SceneContainerScopeExecutionOrder)]
    public class ContainerScope : MonoBehaviour
    {
        public const int SceneContainerScopeExecutionOrder = -1_000_000_000;

        [Tooltip("Drag and drop the parent Scene here. Leave empty to fallback to the Root/Project container.")]
        [SerializeField]
        private SceneReference _parentScene;

        // Returns the Scene name for UnityInjector to process, safe with null
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