using Reflex.Enums;

namespace Reflex.Caching
{
    public abstract class BaseInjectableInfo
    {
        public readonly InjectScope Scope;
        public readonly InjectResolutionMethod ResolutionMethod;

        public BaseInjectableInfo(InjectScope scope, InjectResolutionMethod resolutionMethod)
        {
            Scope = scope;
            ResolutionMethod = resolutionMethod;
        }
    }
}