namespace Reflex.Enums
{
    /// <summary>
    /// Defines when an object instance should be initialized by the DI Container.
    /// </summary>
    public enum Resolution
    {
        /// <summary>
        /// The instance is created only when it is first resolved/requested. 
        /// This is the default and most memory-efficient behavior.
        /// </summary>
        Lazy,

        /// <summary>
        /// The instance is created immediately as soon as the Container is built.
        /// Useful for background services or systems that need to run early without being explicitly called.
        /// </summary>
        Eager,
    }
}