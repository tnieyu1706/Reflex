using UnityEditor;
using Reflex.Components;

namespace Reflex.Editor
{
    [CustomEditor(typeof(MonoBehaviourInstaller))]
    public class MonoBehaviourInstallerEditor : GenericInstallerEditor
    {
        protected override string Title => "MonoBehaviour Bindings";
        protected override string HelpText => "Add MonoBehaviours and select their respective contracts.";
    }
}