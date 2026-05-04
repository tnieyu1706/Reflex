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
                var targetContainer = container.GetTargetContainer(property.Source);

                if (!property.DeepInjectable)
                {
                    property.PropertyInfo.SetValue(instance,
                        targetContainer.Resolve(property.PropertyInfo.PropertyType));
                    return;
                }

                AttributeInjector.Inject(property, targetContainer);
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(property.PropertyInfo, e);
            }
        }
    }
}