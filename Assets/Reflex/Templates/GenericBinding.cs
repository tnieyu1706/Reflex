using System;
using System.Collections.Generic;

namespace Reflex.Templates
{
    [Serializable]
    public class GenericBinding<T>
    {
        public T Target;
        public List<string> Contracts = new List<string>();
    }
}