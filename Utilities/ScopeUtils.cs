using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reflex.Utilities
{
    /// <summary>
    /// Utility class for centralized container and scope naming conventions.
    /// This ensures consistency across the Reflex Debugger and logging.
    /// </summary>
    internal static class ScopeUtils
    {
        /// <summary>
        /// Generates a standardized name for a Scene-level container.
        /// </summary>
        /// <param name="scene">The Unity Scene associated with the container.</param>
        /// <returns>A formatted string combining the scene's name and its hash code.</returns>
        public static string GetSceneContainerName(Scene scene)
        {
            return $"{scene.name} ({scene.GetHashCode()})";
        }

        /// <summary>
        /// Generates a standardized name for a GameObject-level (Local) container.
        /// </summary>
        /// <param name="gameObject">The GameObject associated with the local scope.</param>
        /// <returns>A formatted string combining the GameObject's name and its Instance ID to ensure uniqueness in the hierarchy.</returns>
        public static string GetLocalContainerName(GameObject gameObject)
        {
            // Adding GetInstanceID() helps distinguish objects with the exact same name in the hierarchy
            return $"GameObjectScope ({gameObject.name} - {gameObject.GetInstanceID()})";
        }
    }
}