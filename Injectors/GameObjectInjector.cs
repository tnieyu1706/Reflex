using Reflex.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Reflex.Injectors
{
    internal static class GameObjectInjector
    {
        /// <summary>
        /// Injects dependencies into a single component.
        /// </summary>
        internal static void InjectSingle(Component component, Container container)
        {
            if (component != null)
            {
                AttributeInjector.Inject(component, container);
            }
        }

        /// <summary>
        /// Injects dependencies into all MonoBehaviours on a specific GameObject.
        /// </summary>
        internal static void InjectObject(GameObject gameObject, Container container)
        {
            using (ListPool<MonoBehaviour>.Get(out var injectables))
            {
                gameObject.GetComponents(injectables);
                for (var i = 0; i < injectables.Count; i++)
                {
                    InjectSingle(injectables[i], container);
                }
            }
        }

        /// <summary>
        /// Injects dependencies recursively into the GameObject and its children.
        /// Optimization: Stops traversing down a branch if a LocalScope is encountered.
        /// </summary>
        internal static void InjectRecursive(GameObject gameObject, Container container)
        {
            InjectObject(gameObject, container);

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i).gameObject;

                // [PRUNING] Stop recursion if the child has a LocalScope, 
                // as it manages its own container and injection.
                if (child.TryGetComponent<LocalScope>(out _))
                {
                    continue;
                }

                InjectRecursive(child, container);
            }
        }
    }
}