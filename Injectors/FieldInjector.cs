using System;
using Reflex.Caching;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Exceptions;

namespace Reflex.Injectors
{
    internal static class FieldInjector
    {
        internal static void Inject(InjectableFieldInfo field, object instance, Container container)
        {
            try
            {
                // Quyết định Container dựa trên InjectSource
                var targetContainer = field.Source == InjectSource.Root && Container.RootContainer != null 
                    ? Container.RootContainer 
                    : container;

                field.FieldInfo.SetValue(instance, targetContainer.Resolve(field.FieldInfo.FieldType));
            }
            catch (Exception e)
            {
                throw new FieldInjectorException(field.FieldInfo, e);
            }
        }
    }
}