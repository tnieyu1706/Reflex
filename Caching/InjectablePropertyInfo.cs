using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectablePropertyInfo
    {
        public readonly PropertyInfo PropertyInfo;
        public readonly InjectSource Source;
        public readonly bool DeepInjectable;

        public InjectablePropertyInfo(PropertyInfo propertyInfo, InjectSource source, bool deepInjectable)
        {
            PropertyInfo = propertyInfo;
            Source = source;
            DeepInjectable = deepInjectable;
        }
    }
}