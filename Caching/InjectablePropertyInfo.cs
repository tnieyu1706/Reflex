using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectablePropertyInfo
    {
        public readonly PropertyInfo PropertyInfo;
        public readonly InjectSource Source;

        public InjectablePropertyInfo(PropertyInfo propertyInfo, InjectSource source)
        {
            PropertyInfo = propertyInfo;
            Source = source;
        }
    }
}