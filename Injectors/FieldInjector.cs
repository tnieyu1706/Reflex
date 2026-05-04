using System;
using Reflex.Caching;
using Reflex.Core;
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
                var targetContainer = container.GetTargetContainer(field.Source);

                if (!field.DeepInjectable)
                {
                    field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
                    return;
                }

                AttributeInjector.Inject(instance, container);
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(field.FieldInfo, e);
            }
        }
    }
}