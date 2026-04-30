using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Core
{
    /// <summary>
    /// Acts as a data PROVIDER and CONSUMER at the GameObject level.
    /// Creates a new child Container specifically for this GameObject and its children.
    /// It installs bindings from attached <see cref="IInstaller"/> components and automatically 
    /// injects dependencies recursively into this GameObject and its children.
    /// </summary>
    // Runs slightly after the Scene Container, but before all normal Injectors and Awakes
    [DefaultExecutionOrder(ContainerScope.SceneContainerScopeExecutionOrder + 10)]
    public class LocalScope : MonoBehaviour
    {
        public Container SelfContainer { get; private set; }

        private void Awake()
        {
            // 1. Find the closest parent Scope in the Transform hierarchy. 
            // If no parent transform has a LocalScope, fallback to the Scene Container as the root.
            var parentContainer = gameObject.TryGetClosestLocalContainer(out var localContainer)
                ? localContainer
                : gameObject.scene.GetSceneContainer();

            // 2. Create a new Scope inheriting from the Parent Container
            SelfContainer = parentContainer.Scope(builder =>
            {
                builder.SetName(Reflex.Utilities.ScopeUtils.GetLocalContainerName(gameObject));

                // 3. Get all Installers attached to *this* GameObject to install bindings
                using (ListPool<IInstaller>.Get(out var installers))
                {
                    GetComponents(installers);
                    for (var i = 0; i < installers.Count; i++)
                    {
                        installers[i].InstallBindings(builder);
                    }
                }
            });

            // 4. Automatically trigger Recursive Injection for this branch
            // Since SceneInjector skips this branch (Pruning), we MUST inject recursively here.
            GameObjectInjector.InjectRecursive(gameObject, SelfContainer);
        }

        private void OnDestroy()
        {
            SelfContainer?.Dispose();
        }
    }
}