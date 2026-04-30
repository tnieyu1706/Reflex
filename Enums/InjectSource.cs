namespace Reflex.Enums
{
    /// <summary>
    /// Defines where the dependency should be resolved from.
    /// </summary>
    public enum InjectSource
    {
        /// <summary>
        /// Resolves from the current container (which inherently includes parent bindings).
        /// </summary>
        Default,
        
        /// <summary>
        /// Forces resolution strictly from the Root Container (Project Scope).
        /// Useful when the local scope has an overridden/broken binding.
        /// </summary>
        Root
    }
}