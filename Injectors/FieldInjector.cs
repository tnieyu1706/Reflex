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

                switch (field.ResolutionMethod)
                {
                    // default
                    case InjectResolutionMethod.Inject:
                        field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
                        return;
                    // binding handler
                    case InjectResolutionMethod.Binding:
                        BindField(field, instance, targetContainer);
                        break;
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
                // instantiate
                var result = field.FieldInfo.GetValue(instance)
                             ?? (targetContainer.Resolve(field.FieldInfo.FieldType)
                                 ?? targetContainer.Instantiate(field.FieldInfo.FieldType));
                // binding
                targetContainer.InjectObject(result);
                field.FieldInfo.SetValue(instance, result);
            }
        }
    }
}