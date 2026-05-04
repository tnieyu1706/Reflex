using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectablePropertyInfo : BaseInjectableInfo
    {
        public readonly PropertyInfo PropertyInfo;

        public InjectablePropertyInfo(PropertyInfo propertyInfo, InjectScope scope,
            InjectResolutionMethod resolutionMethod)
            : base(scope, resolutionMethod)
        {
            PropertyInfo = propertyInfo;
        }
    }
}