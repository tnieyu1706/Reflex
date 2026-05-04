using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

namespace Reflex.Components
{
    public class ScriptableObjectsEarlyInjector : MonoBehaviour, IEarlyInjector
    {
        [SerializeField] private List<ScriptableObject> _scriptables = new();

        public void EarlyInjects(Container container)
        {
            foreach (var obj in _scriptables)
            {
                if (obj == null) continue;
                container.InjectObject(obj);
            }
        }
    }
}