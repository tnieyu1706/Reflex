namespace Reflex.Enums
{
    /// <summary>
    /// Defines where the dependency should be resolved from in the Container hierarchy.
    /// </summary>
    public enum InjectSource
    {
        /// <summary>
        /// Resolves from the current container (which inherently includes parent bindings).
        /// This is the default behavior.
        /// </summary>
        Default,

        /// <summary>
        /// Forces resolution strictly from the Local Container (GameObjectScope).
        /// If the current injection context is already Local, it acts like Default.
        /// </summary>
        Local,

        /// <summary>
        /// Forces resolution strictly from the Scene Container.
        /// Useful when a LocalScope overrides a binding, but you need the Scene-level instance.
        /// </summary>
        Scene,
        
        /// <summary>
        /// Forces resolution strictly from the Root Container (Project Scope).
        /// Useful when local/scene scopes have overridden/broken bindings.
        /// </summary>
        Root
    }
}