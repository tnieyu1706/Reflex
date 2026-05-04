using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectableFieldInfo : BaseInjectableInfo
    {
        public readonly FieldInfo FieldInfo;

        public InjectableFieldInfo(FieldInfo fieldInfo, InjectScope scope, InjectResolutionMethod resolutionMethod)
            : base(scope, resolutionMethod)
        {
            FieldInfo = fieldInfo;
        }
    }
}