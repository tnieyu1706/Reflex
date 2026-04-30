using System;
using UnityEngine;
using Reflex.Core;
using Reflex.Injectors;

namespace Reflex.Extensions
{
    public static class ContainerInstantiateExtensions
    {
        /// <summary>
        /// Instantiates a GameObject from a Prefab and injects dependencies before its Awake() method is called.
        /// </summary>
        /// <param name="container">The current container.</param>
        /// <param name="prefab">The prefab to instantiate.</param>
        /// <param name="parent">The parent transform (optional).</param>
        /// <param name="instantiateInWorldSpace">Whether to keep the prefab's world space properties.</param>
        /// <returns>The instantiated GameObject with dependencies injected.</returns>
        public static GameObject InstantiateAndBind(
            this Container container,
            GameObject prefab,
            Transform parent = null,
            bool instantiateInWorldSpace = false)
        {
            // 1. Save the original active state of the prefab
            bool wasActive = prefab.activeSelf;

            // 2. Temporarily deactivate the original prefab to prevent Unity from automatically calling Awake() after Instantiate
            if (wasActive)
            {
                prefab.SetActive(false);
            }

            GameObject instance;
            try
            {
                // 3. Instantiate the clone (the clone will inherit the inactive state, so Awake() will NOT run yet)
                instance = UnityEngine.Object.Instantiate(prefab, parent, instantiateInWorldSpace);
            }
            finally
            {
                // 4. Always ensure the original prefab is reactivated immediately (using try-finally for safety)
                if (wasActive)
                {
                    prefab.SetActive(true);
                }
            }

            // 5. Start injecting dependencies into the clone before it wakes up
            GameObjectInjector.InjectRecursive(instance, container);

            // 6. Reactivate the clone (if the original prefab was active).
            // At this exact step, Unity will begin calling Awake(), Start(), etc., and the injected data is already prepared.
            if (wasActive)
            {
                instance.SetActive(true);
            }

            return instance;
        }

        /// <summary>
        /// Instantiates a Component from a Prefab and injects dependencies before its Awake() method is called.
        /// </summary>
        /// <param name="container">The current container.</param>
        /// <param name="prefab">The Component prefab to instantiate.</param>
        /// <param name="parent">The parent transform (optional).</param>
        /// <param name="instantiateInWorldSpace">Whether to keep the prefab's world space properties.</param>
        /// <returns>The instantiated Component with dependencies injected.</returns>
        public static T InstantiateAndBind<T>(
            this Container container,
            T prefab,
            Transform parent = null,
            bool instantiateInWorldSpace = false) where T : Component
        {
            var instanceObject = container.InstantiateAndBind(prefab.gameObject, parent, instantiateInWorldSpace);
            return instanceObject.GetComponent<T>();
        }

        /// <summary>
        /// Adds a Component of type T to the GameObject and injects dependencies before its Awake() method is called.
        /// WARNING: If the GameObject is currently active, this method will temporarily deactivate it.
        /// This means OnDisable() and OnEnable() will be triggered for all existing components on this GameObject.
        /// </summary>
        /// <param name="container">The current container.</param>
        /// <param name="gameObject">The GameObject to add the component to.</param>
        /// <returns>The newly added and injected Component.</returns>
        public static T AddComponentAndBind<T>(this Container container, GameObject gameObject) where T : Component
        {
            bool wasActive = gameObject.activeSelf;

            // Temporarily deactivate to prevent Awake() from running immediately
            if (wasActive)
            {
                gameObject.SetActive(false);
            }

            T component;
            try
            {
                component = gameObject.AddComponent<T>();
                container.Bind(component);
            }
            finally
            {
                // Reactivate to trigger Awake() with injected dependencies
                if (wasActive)
                {
                    gameObject.SetActive(true);
                }
            }

            return component;
        }

        /// <summary>
        /// Adds a Component of a specific Type to the GameObject and injects dependencies before its Awake() method is called.
        /// WARNING: If the GameObject is currently active, this method will temporarily deactivate it.
        /// This means OnDisable() and OnEnable() will be triggered for all existing components on this GameObject.
        /// </summary>
        /// <param name="container">The current container.</param>
        /// <param name="gameObject">The GameObject to add the component to.</param>
        /// <param name="componentType">The Type of the component to add.</param>
        /// <returns>The newly added and injected Component.</returns>
        public static Component AddComponentAndBind(this Container container, GameObject gameObject, Type componentType)
        {
            bool wasActive = gameObject.activeSelf;

            // Temporarily deactivate to prevent Awake() from running immediately
            if (wasActive)
            {
                gameObject.SetActive(false);
            }

            Component component;
            try
            {
                component = gameObject.AddComponent(componentType);
                container.Bind(component);
            }
            finally
            {
                // Reactivate to trigger Awake() with injected dependencies
                if (wasActive)
                {
                    gameObject.SetActive(true);
                }
            }

            return component;
        }
    }
}