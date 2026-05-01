using UnityEditor;
using Reflex.Components;

namespace Reflex.Editor
{
    [CustomEditor(typeof(MonoBehaviourInstaller))]
    public class MonoBehaviourInstallerEditor : GenericInstallerEditor
    {
        protected override string Title => "MonoBehaviour Bindings";
    }
}