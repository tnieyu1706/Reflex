using Reflex.Core;
using UnityEngine;

namespace Reflex.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Retrieves the closest container for this GameObject.
        /// It traverses up the hierarchy to find a GameObjectScope.
        /// If none is found, it falls back to the Scene's Container.
        /// </summary>
        public static bool TryGetClosestLocalContainer(this GameObject gameObject, out Container container)
        {
            var goScope = gameObject.GetComponentInParent<LocalScope>(true);
            if (goScope != null && goScope.SelfContainer != null)
            {
                container = goScope.SelfContainer;
                return true;
            }

            container = null;
            return false;
        }

        /// <summary>
        /// Retrieves the closest dependency injection container for this GameObject.
        /// It traverses up the hierarchy to find a LocalScope. 
        /// If no local scope is found, it falls back to the Scene's container, and ultimately to the Root container.
        /// </summary>
        /// <param name="gameObject">The target GameObject to find the container for.</param>
        /// <returns>The closest Container available in the hierarchy.</returns>
        public static Container GetClosestContainer(this GameObject gameObject)
        {
            var goScope = gameObject.GetComponentInParent<LocalScope>(true);
            if (goScope != null && goScope.SelfContainer != null)
            {
                return goScope.SelfContainer;
            }

            var container = gameObject.scene.GetSceneContainer();
            return container ?? Container.RootContainer;
        }
    }
}