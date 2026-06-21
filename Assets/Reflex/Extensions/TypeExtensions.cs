using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Reflex.Extensions
{
    internal static class TypeExtensions
    {
        internal static bool IsEnumerable(this Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GenericTypeArguments.Single();
                return true;
            }

            elementType = null;
            return false;
        }
        
        internal static bool TryGetConstructors(this Type type, out ConstructorInfo[] constructors)
        {
            constructors = type.GetConstructors();
            return constructors.Length > 0;
        }
        
        internal static string GetName(this Type type)
        {
            if (type.IsGenericType)
            {
                var outerTypeName = type.Name!.Split('`').First();
                var innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetName));
                return $"{outerTypeName}<{innerTypeNames}>";
            }

            return type.Name;
        }
        
        internal static string GetFullName(this Type type)
        {
            if (type.IsGenericType)
            {
                var outerTypeName = type.FullName!.Split('`').First();
                var innerTypeNames = string.Join(", ", type.GenericTypeArguments.Select(GetFullName));
                return $"{outerTypeName}<{innerTypeNames}>";
            }

            return type.FullName;
        }
        
        internal static HashSet<Type> GetAllParentTypes(this Type type)
        {
            HashSet<Type> result = new();

            Type current = type.BaseType;
            while (current != null)
            {
                result.Add(current);
                current = current.BaseType;
            }

            foreach (var i in type.GetInterfaces())
            {
                result.Add(i);
            }

            return result;
        }

        internal static HashSet<Type> GetAllParentTypesWithSelf(this Type type)
        {
            var result = GetAllParentTypes(type);
            result.Add(type);
            return result;
        }
    }
}