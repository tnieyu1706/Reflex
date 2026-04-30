using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Exceptions;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Logging;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Container : IDisposable
    {
        public static Container RootContainer { get; internal set; }
        public string Name { get; }
        public Container Parent { get; }
        internal List<Container> Children { get; } = new();
        internal Dictionary<Type, List<IResolver>> ResolversByContract { get; }
        internal DisposableCollection Disposables { get; }
#if UNITY_EDITOR
        internal static readonly List<Container> RootContainers = new();
#endif

        [ThreadStatic] private static Stack<Type> _resolutionChain;
        private static Stack<Type> ResolutionChain => _resolutionChain ??= new Stack<Type>();

        internal Container(string name, Container parent, Dictionary<Type, List<IResolver>> resolversByContract,
            DisposableCollection disposables)
        {
            Diagnosis.RegisterBuildCallSite(this);
            Name = name;
            Parent = parent;
            Parent?.Children.Add(this);
            ResolversByContract = resolversByContract;
            Disposables = disposables;
            OverrideSelfInjection();

#if UNITY_EDITOR
            if (parent == null)
            {
                RootContainers.Add(this);
            }
#endif
        }

        public bool HasBinding<T>() => HasBinding(typeof(T));

        public bool HasBinding(Type type) => ResolversByContract.ContainsKey(type);

        public void Dispose()
        {
            // 1. Snapshot the children list and clear the original list immediately.
            // This prevents O(N^2) performance issues and "Collection was modified" exceptions 
            // when children inevitably call 'Parent?.Children.Remove(this)' inside their own Dispose method.
            var childrenSnapshot = Children.ToArray();
            Children.Clear();

            // 2. Safely dispose children in reverse order (Standard DI tear-down behavior)
            for (var i = childrenSnapshot.Length - 1; i >= 0; i--)
            {
                childrenSnapshot[i].Dispose();
            }

            // 3. Detach from parent
            Parent?.Children.Remove(this);

#if UNITY_EDITOR
            if (Parent == null)
            {
                RootContainers.Remove(this);
            }
#endif

            // 4. Cleanup internal state
            ResolversByContract.Clear();
            Disposables.Dispose();

            ReflexLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(Action<ContainerBuilder> extend = null)
        {
            var builder = new ContainerBuilder().SetParent(this);
            extend?.Invoke(builder);
            return builder.Build();
        }

        /// <summary>
        /// Instantiates a new object and automatically injects all its dependencies.
        /// Performs both Instantiate (Constructor) and Bind (Field/Property/Method injection) sequentially.
        /// </summary>
        public T Construct<T>() => (T)Construct(typeof(T));

        /// <summary>
        /// Instantiates a new object from the provided Type and automatically injects all its dependencies.
        /// </summary>
        public object Construct(Type concrete)
        {
            var instance = Instantiate(concrete);
            Bind(instance);
            return instance;
        }

        /// <summary>
        /// Injects dependencies into an existing instance's fields, properties, and methods marked with [Inject].
        /// </summary>
        public void Bind(object instance)
        {
            AttributeInjector.Inject(instance, this);
        }

        /// <summary>
        /// Creates a new instance via its constructor.
        /// Prioritizes the constructor marked with the [ReflexConstructor] attribute.
        /// </summary>
        public T Instantiate<T>() => (T)Instantiate(typeof(T));

        /// <summary>
        /// Creates a new instance from the provided Type via its constructor.
        /// </summary>
        public object Instantiate(Type concrete)
        {
            return ConstructorInjector.Construct(concrete, this);
        }

        /// <summary>
        /// Resolves an instance of the specified type from the container.
        /// Tracks the resolution path to detect and prevent circular dependencies.
        /// </summary>
        public object Resolve(Type type)
        {
            if (type.IsEnumerable(out var elementType))
            {
                return All(elementType).CastDynamic(elementType);
            }

            // 1. Check for circular dependency loop
            if (ResolutionChain.Contains(type))
            {
                throw new CircularDependencyException(ResolutionChain, type);
            }

            // 2. Push to monitor stack
            ResolutionChain.Push(type);

            try
            {
                // 3. Get resolvers and resolve using the last registered resolver.
                var resolvers = GetResolvers(type);
                return resolvers.Last().Resolve(this);
            }
            finally
            {
                // 4. Pop after resolution attempt to maintain correct state.
                ResolutionChain.Pop();
            }
        }

        public TContract Resolve<TContract>() => (TContract)Resolve(typeof(TContract));

        public object Single(Type type) => GetResolvers(type).Single().Resolve(this);

        public TContract Single<TContract>() => (TContract)Single(typeof(TContract));

        public bool TryGetResolver(Type contract, out IResolver result)
        {
            if (ResolversByContract.TryGetValue(contract, out var resolvers))
            {
                result = resolvers.Single();
                return true;
            }

            result = null;
            return false;
        }

        public bool TryGetResolver<TContract>(out IResolver result) => TryGetResolver(typeof(TContract), out result);

        public IEnumerable<object> All(Type contract)
        {
            return ResolversByContract.TryGetValue(contract, out var resolvers)
                ? resolvers.Select(resolver => resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<object>();
        }

        public IEnumerable<TContract> All<TContract>()
        {
            return ResolversByContract.TryGetValue(typeof(TContract), out var resolvers)
                ? resolvers.Select(resolver => (TContract)resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<IResolver> GetResolvers(Type contract)
        {
            if (ResolversByContract.TryGetValue(contract, out var resolvers))
            {
                return resolvers;
            }

            throw new UnknownContractException(contract);
        }

        private void OverrideSelfInjection()
        {
            ResolversByContract[typeof(Container)] = new List<IResolver> { new SingletonValueResolver(this) };
        }
    }
}