using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Exceptions;
using Reflex.Extensions;

namespace Reflex.Injectors
{
    internal static class PropertyInjector
    {
        internal static void Inject(InjectablePropertyInfo property, object instance, Container container)
        {
            try
            {
                // Determine the Container based on InjectSource (Using extension method)
                var targetContainer = container.GetTargetContainer(property.Source);

                property.PropertyInfo.SetValue(instance, targetContainer.Resolve(property.PropertyInfo.PropertyType));
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(property.PropertyInfo, e);
            }
        }
    }
}