using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

namespace Reflex.Components
{
    public class UnityObjectsEarlyInjector : MonoBehaviour, IEarlyInjector
    {
        [SerializeField] private List<Object> _unityObjects = new List<UnityEngine.Object>();

        public void EarlyInjects(Container container)
        {
            foreach (var obj in _unityObjects)
            {
                if (obj == null) continue;
                container.InjectObject(obj);
            }
        }
    }
}