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
    }
}