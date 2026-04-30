using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflex.Exceptions
{
    /// <summary>
    /// Exception thrown when a circular dependency is detected during the resolution of an object graph.
    /// This prevents StackOverflow exceptions and provides a clear trace of the dependency chain.
    /// </summary>
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(IEnumerable<Type> resolutionChain, Type targetType)
            : base(BuildMessage(resolutionChain, targetType))
        {
        }

        private static string BuildMessage(IEnumerable<Type> resolutionChain, Type targetType)
        {
            var chain = string.Join(" -> ", resolutionChain.Select(t => t.Name).Reverse());
            return $"Circular dependency detected: {chain} -> {targetType.Name}. Please check your bindings and object graph design.";
        }
    }
}