using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;
using Reflex.Extensions;

namespace Reflex.Injectors
{
    internal static class FieldInjector
    {
        internal static void Inject(InjectableFieldInfo field, object instance, Container container)
        {
            try
            {
                var targetContainer = container.GetTargetContainer(field.Scope);

                // default
                if (field.ResolutionMethod == InjectResolutionMethod.Inject)
                {
                    field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
                    return;
                }

                // binding handler
                if (field.ResolutionMethod == InjectResolutionMethod.Binding)
                {
                    BindField(field, instance, targetContainer);
                }
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(field.FieldInfo, e);
            }
        }

        private static void BindField(InjectableFieldInfo field, object instance, Container targetContainer)
        {
            if (field.FieldInfo.FieldType.IsValueType)
            {
                field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
            }
            else
            {
                var fieldValue = field.FieldInfo.GetValue(instance);
                if (fieldValue == null)
                {
                    field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
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