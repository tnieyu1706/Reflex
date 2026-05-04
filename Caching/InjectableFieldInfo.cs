using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectableFieldInfo
    {
        public readonly FieldInfo FieldInfo;
        public readonly InjectSource Source;
        public readonly bool DeepInjectable;

        public InjectableFieldInfo(FieldInfo fieldInfo, InjectSource source, bool deepInjectable)
        {
            FieldInfo = fieldInfo;
            Source = source;
            DeepInjectable = deepInjectable;
        }
    }
}