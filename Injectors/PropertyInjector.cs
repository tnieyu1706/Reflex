using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Enums;
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
                var targetContainer = container.GetTargetContainer(property.Scope);

                // default
                if (property.ResolutionMethod == InjectResolutionMethod.Inject)
                {
                    property.PropertyInfo.SetValue(instance,
                        targetContainer.Resolve(property.PropertyInfo.PropertyType));
                    return;
                }

                // binding handler
                if (property.ResolutionMethod == InjectResolutionMethod.Binding)
                {
                    BindProperty(property, instance, targetContainer);
                }
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(property.PropertyInfo, e);
            }
        }

        private static void BindProperty(InjectablePropertyInfo property, object instance, Container targetContainer)
        {
            if (property.PropertyInfo.PropertyType.IsValueType)
            {
                property.PropertyInfo.SetValue(instance, targetContainer.Resolve(property.PropertyInfo.PropertyType));
            }
            else
            {
                var fieldValue = property.PropertyInfo.GetValue(instance);
                if (fieldValue == null)
                {
                    property.PropertyInfo.SetValue(instance,
                        targetContainer.Resolve(property.PropertyInfo.PropertyType));
                }
                else
                {
                    // inject only
                    targetContainer.InjectObject(fieldValue);
                }
            }
        }
    }
}