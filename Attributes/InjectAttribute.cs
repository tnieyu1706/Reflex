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
        public InjectScope Scope { get; set; }
        
        public InjectResolutionMethod ResolutionMethod { get; set; }

        public InjectAttribute(
            InjectScope scope = InjectScope.Default,
            InjectResolutionMethod resolutionMethod = InjectResolutionMethod.Inject)
        {
            Scope = scope;
            ResolutionMethod = resolutionMethod;
        }
    }
}