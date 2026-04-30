using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectableFieldInfo
    {
        public readonly FieldInfo FieldInfo;
        public readonly InjectSource Source;

        public InjectableFieldInfo(FieldInfo fieldInfo, InjectSource source)
        {
            FieldInfo = fieldInfo;
            Source = source;
        }
    }
}