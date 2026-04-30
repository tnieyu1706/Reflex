using System;
using Reflex.Core;
using Reflex.Extensions;
using Reflex.Injectors;
using UnityEngine;

namespace Reflex.Components
{
    /// <summary>
    /// Acts as a data CONSUMER at the GameObject level.
    /// Finds the closest available Container (from a parent LocalScope, or fallbacks to the Scene Container) 
    /// and injects dependencies into the attached GameObject based on the chosen strategy.
    /// It does NOT create any new Containers.
    /// </summary>
    [DefaultExecutionOrder(ContainerScope.SceneContainerScopeExecutionOrder + 100)] // +100 instead of +1 to leave room for other user custom components
    internal sealed class LocalSelfInjector : MonoBehaviour
    {
        [SerializeField] private InjectionStrategy _injectionStrategy = InjectionStrategy.Recursive;

        private void Awake()
        {
            // CHANGES HERE: Instead of getting directly from the Scene, get from the closest container in the hierarchy
            if (!gameObject.TryGetClosestLocalContainer(out var container)) return;

            switch (_injectionStrategy)
            {
                case InjectionStrategy.Single:
                    GameObjectInjector.InjectSingle(gameObject, container);
                    break;
                case InjectionStrategy.Object:
                    GameObjectInjector.InjectObject(gameObject, container);
                    break;
                case InjectionStrategy.Recursive:
                    GameObjectInjector.InjectRecursive(gameObject, container);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_injectionStrategy.ToString());
            }
        }

        private enum InjectionStrategy
        {
            Single,
            Object,
            Recursive
        }
    }
}