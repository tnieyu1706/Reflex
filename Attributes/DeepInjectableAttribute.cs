using System;

namespace Reflex.Attributes
{
    /// <summary>
    /// Flags a field or property to instruct Reflex to recursively inject dependencies into its nested members.
    /// Typically used alongside [Inject] on Unity-serialized objects (e.g., [Serializable] classes).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DeepInjectableAttribute : Attribute
    {
    }
}