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

                switch (property.ResolutionMethod)
                {
                    // default
                    case InjectResolutionMethod.Inject:
                        property.PropertyInfo.SetValue(instance,
                            targetContainer.Resolve(property.PropertyInfo.PropertyType));
                        return;
                    // binding handler
                    case InjectResolutionMethod.Binding:
                        BindProperty(property, instance, targetContainer);
                        break;
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
                // instantiate
                var result = property.PropertyInfo.GetValue(instance)
                             ?? (targetContainer.Resolve(property.PropertyInfo.PropertyType)
                                 ?? targetContainer.Instantiate(property.PropertyInfo.PropertyType));

                // binding
                targetContainer.InjectObject(result);
                property.PropertyInfo.SetValue(instance, result);
            }
        }
    }
}