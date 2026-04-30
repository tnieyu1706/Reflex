using Reflex.Core;
using UnityEngine.SceneManagement;

namespace Reflex.Injectors
{
    internal static class SceneInjector
    {
        internal static void Inject(Scene scene, Container container)
        {
            foreach (var rootObject in scene.GetRootGameObjects())
            {
                // [PRUNING] Skip branches that define their own LocalScope.
                // The LocalScope itself will handle recursive injection for its branch.
                if (rootObject.TryGetComponent<LocalScope>(out _))
                {
                    continue;
                }

                GameObjectInjector.InjectRecursive(rootObject, container);
            }
        }
    }
}