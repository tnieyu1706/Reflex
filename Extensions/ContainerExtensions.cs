using System.Runtime.CompilerServices;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Logging;
using Reflex.Injectors;

namespace Reflex.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly ConditionalWeakTable<Container, ContainerDebugProperties> _containerDebugProperties =
            new();

        internal static ContainerDebugProperties GetDebugProperties(this Container container)
        {
            return _containerDebugProperties.GetOrCreateValue(container);
        }

        /// <summary>
        /// Resolves the correct target container based on the requested InjectSource.
        /// </summary>
        internal static Container GetTargetContainer(this Container currentContainer, InjectSource source)
        {
            switch (source)
            {
                case InjectSource.Root:
                    return Container.RootContainer ?? currentContainer;

                case InjectSource.Scene:
                    var node = currentContainer;

                    // Traverse up to find the exact container registered to a Scene
                    while (node != null)
                    {
                        // Check if the current node is in the Scene Containers list
                        if (UnityInjector.ContainersPerScene.ContainsValue(node))
                        {
                            return node;
                        }

                        node = node.Parent;
                    }

                    // Fallback to current if no scene container found (e.g. isolated testing context)
                    return currentContainer;

                case InjectSource.Local:
                case InjectSource.Default:
                default:
                    // Local and Default simply use the context's current container
                    return currentContainer;
            }
        }
    }
}