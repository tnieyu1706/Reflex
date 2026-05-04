namespace Reflex.Core
{
    public interface IEarlyInjector
    {
        void EarlyInjects(Container container);
    }
}