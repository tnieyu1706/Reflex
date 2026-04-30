using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class PropertyInjector
    {
        internal static void Inject(InjectablePropertyInfo property, object instance, Container container)
        {
            try
            {
                // Quyết định Container dựa trên InjectSource
                var targetContainer = property.Source == InjectSource.Root && Container.RootContainer != null 
                    ? Container.RootContainer 
                    : container;

                property.PropertyInfo.SetValue(instance, targetContainer.Resolve(property.PropertyInfo.PropertyType));
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(property.PropertyInfo, e);
            }
        }
    }
}