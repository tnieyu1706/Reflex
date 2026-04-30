using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectableMethodInfo
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInfo[] Parameters;
        public readonly InjectSource Source;

        public InjectableMethodInfo(MethodInfo methodInfo, InjectSource source)
        {
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
            Source = source;
        }
    }
}