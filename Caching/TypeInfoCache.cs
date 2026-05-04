using System;
using System.Collections.Generic;
using System.Reflection;
using Reflex.Attributes;
using UnityEngine.Pool;

namespace Reflex.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                           BindingFlags.DeclaredOnly;

        private static readonly Dictionary<Type, TypeAttributeInfo> _cache = new();

        internal static TypeAttributeInfo Get(Type type)
        {
            if (!_cache.TryGetValue(type, out var info))
            {
                info = Generate(type);
                _cache.Add(type, info);
            }

            return info;
        }

        internal static TypeAttributeInfo Generate(Type type)
        {
            using var pooled1 = ListPool<InjectableFieldInfo>.Get(out var fieldList);
            using var pooled2 = ListPool<InjectablePropertyInfo>.Get(out var propertyList);
            using var pooled3 = ListPool<InjectableMethodInfo>.Get(out var methodList);

            while (type != null && type != typeof(object))
            {
                foreach (var field in type.GetFields(Flags))
                {
                    var attribute = field.GetCustomAttribute<InjectAttribute>();
                    if (attribute != null)
                    {
                        fieldList.Add(
                            new InjectableFieldInfo(field, attribute.Scope, attribute.ResolutionMethod)
                        );
                    }
                }

                foreach (var property in type.GetProperties(Flags))
                {
                    if (property.CanWrite)
                    {
                        var attribute = property.GetCustomAttribute<InjectAttribute>();
                        if (attribute != null)
                        {
                            propertyList.Add(
                                new InjectablePropertyInfo(property, attribute.Scope, attribute.ResolutionMethod)
                            );
                        }
                    }
                }

                foreach (var method in type.GetMethods(Flags))
                {
                    var attribute = method.GetCustomAttribute<InjectAttribute>();
                    if (attribute != null)
                    {
                        methodList.Add(new InjectableMethodInfo(method, attribute.Scope));
                    }
                }

                type = type.BaseType;
            }

            return new TypeAttributeInfo(fieldList.ToArray(), propertyList.ToArray(), methodList.ToArray());
        }
    }
}