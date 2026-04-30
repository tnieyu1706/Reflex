namespace Reflex.Enums
{
    /// <summary>
    /// Defines the lifespan of an object instance created by the DI Container.
    /// </summary>
    public enum Lifetime
    {
        /// <summary>
        /// A single instance is created and shared across the container and all its children.
        /// </summary>
        Singleton,

        /// <summary>
        /// A new instance is created every time it is resolved. 
        /// Note: Cannot be used with Resolution.Eager.
        /// </summary>
        Transient,

        /// <summary>
        /// A single instance is created per container. 
        /// Child containers will create their own unique instance instead of inheriting from the parent.
        /// </summary>
        Scoped,
    }
}