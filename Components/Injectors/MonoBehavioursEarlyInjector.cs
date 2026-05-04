using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

namespace Reflex.Components
{
    public class MonoBehavioursEarlyInjector : MonoBehaviour, IEarlyInjector
    {
        [SerializeField] private List<MonoBehaviour> _monoBehaviours = new();

        public void EarlyInjects(Container container)
        {
            foreach (var behaviour in _monoBehaviours)
            {
                if (behaviour == null) continue;
                container.InjectObject(behaviour);
            }
        }
    }
}