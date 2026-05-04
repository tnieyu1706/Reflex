namespace Reflex.Enums
{
    /// <summary>
    /// Defines how a dependency should be resolved during the injection process.
    /// </summary>
    public enum InjectResolutionMethod
    {
        /// <summary>
        /// Resolves the dependency by directly injecting it into the target instance.
        /// </summary>
        Inject,

        /// <summary>
        /// Resolves the dependency through the binding system, typically using registered contracts.
        /// </summary>
        Binding,
    }
}