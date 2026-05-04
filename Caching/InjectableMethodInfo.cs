using System.Reflection;
using Reflex.Enums;

namespace Reflex.Caching
{
    internal sealed class InjectableMethodInfo
    {
        public readonly MethodInfo MethodInfo;
        public readonly ParameterInfo[] Parameters;
        public readonly InjectScope Scope;

        public InjectableMethodInfo(MethodInfo methodInfo, InjectScope scope)
        {
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
            Scope = scope;
        }
    }
}