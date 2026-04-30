using System;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

namespace Reflex.Components
{
    [Serializable]
    public class GenericBinding<T> where T : UnityEngine.Object
    {
        public T Target;
        [HideInInspector]
        public List<string> Contracts = new List<string>();
#if UNITY_EDITOR
        [HideInInspector]
        public bool _isExpanded = true;
#endif
    }

    public abstract class GenericInstaller<T> : MonoBehaviour, IInstaller where T : UnityEngine.Object
    {
        [SerializeField] private List<GenericBinding<T>> _bindings = new List<GenericBinding<T>>();

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            foreach (var binding in _bindings)
            {
                if (binding.Target == null || binding.Contracts.Count == 0)
                {
                    continue;
                }

                var contracts = new List<Type>();
                foreach (var typeName in binding.Contracts)
                {
                    var type = Type.GetType(typeName);
                    if (type != null)
                    {
                        contracts.Add(type);
                    }
                    else
                    {
                        Debug.LogWarning($"[Reflex] Cannot resolve contract type: {typeName} for target: {binding.Target.name}");
                    }
                }

                if (contracts.Count > 0)
                {
                    containerBuilder.RegisterValue(binding.Target, contracts.ToArray());
                }
            }
        }
    }
}