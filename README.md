Ultra-fast, minimal, and powerful Dependency Injection for Unity (Custom Version)

Reflex is a Dependency Injection (DI) framework for Unity that decouples object creation from usage. This custom version extends the original Reflex capabilities with features like LocalScope, Inject Source, and powerful Instantiate Extensions.

🌟 Key Features (Custom Version)

In addition to the core features (Source Generation, Immutable Container, High Performance), this version includes significant improvements:

LocalScope (GameObject-level Container): Allows creating a specific container for a GameObject and its children. Extremely useful for complex UI Panels or isolated systems within a Scene.

InjectSource: Precisely control the dependency resolution source (Root, Scene, Local) via Attributes.

Instantiate & Bind Extensions: Support for instantiating Prefabs/Components and performing injection immediately before the Awake() method runs.

Visual Installers: Specialized installers for MonoBehaviour and ScriptableObject with an intuitive Inspector interface for selecting Contracts (Interfaces/Base Classes).

Scene Hierarchy Inheritance: Establish parent-child relationships between Scene Containers directly through the ContainerScope Inspector.

🛠 Core Changes & Usage

1. LocalScope - GameObject-level Container

LocalScope acts as a "Mini-Container" within the Transform hierarchy.

It automatically creates a child container inheriting from the nearest container (usually the Scene Container).

Automatically injects into itself and all child GameObjects.

Usage: Attach the LocalScope component to a GameObject. Add Installers to that same GameObject to register specific bindings for this hierarchy.

2. Specifying Injection Source (InjectSource)

You can override the default resolution logic by specifying the source via the InjectAttribute:

public class MyService : MonoBehaviour
{
    // Resolves the instance from the Scene Container, even if a LocalScope overrides it
    [Inject(Source = InjectSource.Scene)] 
    private ILogger _sceneLogger;

    // Only searches within the Root (Project) Container
    [Inject(Source = InjectSource.Root)] 
    private IConfig _globalConfig;
}


3. Dynamic Object Creation (Extensions)

Solves the issue of dependencies not being ready during Awake. New extension methods ensure injection is complete before the object "awakens":

// Instantiate prefab and inject immediately
var enemy = container.InstantiateAndBind(enemyPrefab, spawnPoint);

// Add component and inject before Awake runs
var health = container.AddComponentAndBind<Health>(gameObject);


4. Visual Installers (Generic Installers)

Use MonoBehaviourInstaller or ScriptableObjectInstaller to register instances available in the Scene or Assets:

Drag and drop objects into the Bindings list.

The system automatically lists all valid Interfaces and Base Classes for you to toggle (Contracts).

5. Scene Hierarchy via Inspector

In ContainerScope (SceneScope), you can now drag and drop a Scene Asset into the Parent Scene field.

This allows the current Scene to inherit all dependencies from the defined Parent Scene.

Strongly supports Multi-scene architectures (e.g., a "UI" Scene inheriting from a "Core" Scene).

🎬 Execution Order

The custom version adjusts execution order to ensure stability:

ContainerScope (Scene): -1,000,000,000 (Initializes the Scene container).

LocalScope: SceneContainerScope + 10 (Initializes the GameObject container after the Scene).

Standard Components: Execute after being fully injected.

🔍 Diagnosis & Debugging

This custom version integrates a Diagnosis system that records the CallStack for both Binding and Resolution points.

Open Window → Analysis → Reflex Debugger to view details.

Enable by adding the REFLEX_DEBUG scripting define symbol in Player Settings.

📜 License

This version inherits the MIT license. Contributions based on the original work by Gustavo Santos are welcome.
