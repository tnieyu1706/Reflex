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
                // Determine the Container based on InjectSource (Using extension method)
                var targetContainer = container.GetTargetContainer(field.Source);

                field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(field.FieldInfo, e);
            }
        }
    }
}