using System;
using JetBrains.Annotations;
using Reflex.Enums;

namespace Reflex.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// Specifies the target container to resolve this dependency from.
        /// Defaults to the local scope (which includes inherited bindings).
        /// </summary>
        public InjectSource Source { get; set; }

        public InjectAttribute(InjectSource source = InjectSource.Default)
        {
            Source = source;
        }
    }
}