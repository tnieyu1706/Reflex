# ReflexOrigin

A custom fork of [Reflex](https://github.com/gustavopsantos/Reflex) — a minimal dependency injection framework for Unity.

This fork extends the original with multi-scene parent-child container support, visual installer templates, and Unity object instantiation utilities.

## What's Different from Upstream

| Feature | Description |
|---------|-------------|
| **`Create()` / `Bind()` separation** | Constructor injection (`Create`) and attribute injection (`Bind`) are now separate methods, with `Construct()` as the combined shorthand. |
| **Scene-scoped container hierarchy** | `ContainerScope` now has a `SceneReference` field to define a parent scene, enabling proper container parent-child chains in multi-scene projects. |
| **Unity instantiation extensions** | `Container.InstantiatePrefab()` and `Container.AddComponentAndBind()` — inject dependencies before `Awake()` runs. |
| **Visual installer templates** | `MonoBehaviourInstaller` and `ScriptableObjectInstaller` — drag-and-drop serialized binding in the Inspector. |
| **Custom editor tools** | `ContainerScopeEditor` with parent scene selector + `GenericBindingDrawer` for toggle-based contract selection. |

## Installation

Add via Unity Package Manager (Git URL):

```
https://github.com/tnieyu1706/Reflex.git?path=Assets/Reflex
```

Or copy `Assets/Reflex/` directly into your project.

## Quick Start

```csharp
public class GameInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder builder)
    {
        builder.RegisterType<PlayerService>(Lifetime.Singleton);
        builder.RegisterType<InputHandler>(Lifetime.Transient);
        builder.RegisterValue(new SettingsData(), typeof(ISettings));
    }
}
```

Attach `ContainerScope` to a GameObject in your scene, add your installers as children, and use `[Inject]` attributes on fields/properties/methods:

```csharp
public class Player : MonoBehaviour
{
    [Inject] private PlayerService _playerService;

    private void Start()
    {
        _playerService.DoSomething();
    }
}
```

## Custom Features in Detail

### 1. `Create` / `Bind` / `Construct`

The original `Construct()` was split into distinct phases for fine-grained control:

- **`container.Create<T>()`** — resolves constructor arguments and instantiates the type (no attribute injection).
- **`container.Bind(existingInstance)`** — injects `[Inject]` fields, properties, and methods into an already-existing object.
- **`container.Construct<T>()`** — calls `Create` followed by `Bind` (original behavior).

### 2. Multi-Scene Container Hierarchy

In `ContainerScope`, set the **Parent Scene** field via drag-and-drop (`SceneReference`). When the scene loads, `UnityInjector` resolves the parent scene's container and creates the new scene container as its child.

```
RootContainer
├── Scene A Container (parent: RootContainer)
│   └── Scene A-Child Container (parent: Scene A)  ← parent scene set via ContainerScope
├── Scene B Container (parent: RootContainer)
└── Scene C Container (parent: RootContainer)
```

If the parent scene isn't loaded, it gracefully falls back to `RootContainer` — useful for isolated scene testing.

### 3. Prefab & Component Instantiation

```csharp
// Instantiate a prefab — Awake() runs after injection
var player = container.InstantiatePrefab(playerPrefab, parentTransform);

// Add a component at runtime — Awake() runs after injection
var health = container.AddComponentAndBind<Health>(gameObject);
```

Both methods temporarily deactivate the object during injection so that `Awake()`/`Start()` see fully injected dependencies.

### 4. Visual Installer Templates

Two drag-and-drop installers available via `GameObject > Reflex` menu:

```
[MonoBehaviour Installer]   → Drag MonoBehaviours from the scene
[Scriptable Object Installer] → Drag ScriptableObjects from assets
```

Configure bindings in the Inspector — select target object, toggle which contract types to register it as.

## Core API

### Container

```csharp
container.Scope(extend)          // Create child container (scoped lifetime)
container.Create<T>()            // Constructor injection only
container.Bind(instance)         // Attribute injection only
container.Construct<T>()         // Create + Bind
container.Resolve<T>()           // Resolve from container hierarchy
container.Single<T>()            // Resolve expecting exactly one binding
container.All<T>()               // Enumerate all bindings
```

### ContainerBuilder

```csharp
// Registration
builder.RegisterType<Service>(Lifetime.Singleton, Resolution.Lazy);
builder.RegisterValue(instance, typeof(IService));
builder.RegisterFactory(c => new Service(c.Resolve<IDep>()), Lifetime.Transient);

// Parent & naming (fluent)
builder.SetName("MyContainer").SetParent(parentContainer);
```

### Lifetime & Resolution

| Lifetime | Behavior |
|----------|----------|
| `Singleton` | One instance per declaring container |
| `Transient` | New instance on every resolve |
| `Scoped` | One instance per resolving container (child scopes) |

| Resolution | Behavior |
|------------|----------|
| `Lazy` | Instance created on first resolve |
| `Eager` | Instance created at container build time |

## Project Structure

```
Assets/Reflex/
├── Core/               — Container, ContainerScope, ContainerBuilder, Binding, IInstaller
├── Injectors/          — Constructor/Attribute/GameObject/Scene/Unity injectors
├── Resolvers/          — Singleton, Transient, Scoped resolvers (type, value, factory)
├── Extensions/         — ContainerUnityObjectExtensions, SceneExtensions, TypeExtensions
├── Templates/          — GenericInstaller, MonoBehaviourInstaller, ScriptableObjectInstaller
├── Attributes/         — [Inject], [ReflexConstructor], [SourceGeneratorInjectable]
├── Caching/            — Reflection caches (TypeInfoCache, TypeConstructionInfoCache)
├── Reflectors/         — Activator factories (Mono via Expression, IL2CPP)
├── Editor/             — Debugger window, custom editors, property drawers, linker
├── Logging/            — ReflexLogger
├── Utilities/          — SceneReference, ScriptingBackend
└── Pooling/            — SizeSpecificArrayPool
```

## Performance

- Reflection results cached in `TypeInfoCache` / `TypeConstructionInfoCache` (never recached per type)
- `SizeSpecificArrayPool` for zero-allocation parameter arrays
- `ConditionalWeakTable<Container, object>` for scoped resolver storage (no manual cleanup)
- Roslyn source generator (optional, via `[SourceGeneratorInjectable]`) replaces reflection with compile-time resolved calls
- IL2CPP support with `link.xml` preservation

## Requirements

- Unity 2021.1+
- No external dependencies

## License

MIT — original by Gustavo Santos, fork maintained by TNieAccStudy.
