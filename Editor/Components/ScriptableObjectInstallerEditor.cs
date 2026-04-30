using UnityEditor;
using Reflex.Components;

namespace Reflex.Editor
{
    [CustomEditor(typeof(ScriptableObjectInstaller))]
    public class ScriptableObjectInstallerEditor : GenericInstallerEditor
    {
        protected override string Title => "Scriptable Object Bindings";
        protected override string HelpText => "Add ScriptableObjects and select their respective contracts.";
    }
}