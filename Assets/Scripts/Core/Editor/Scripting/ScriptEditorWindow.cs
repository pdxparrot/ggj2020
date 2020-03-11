using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Editor.NodeEditor;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptEditorWindow : NodeEditorWindow
    {
        private const string MainStyleSheet = "ScriptEditorWindow/Main";
        private const string WindowLayout = "ScriptEditorWindow/Window";

        [MenuItem("PDX Party Parrot/Core/Script Editor...")]
        public static void ShowWindow()
        {
            ScriptEditorWindow window = GetWindow<ScriptEditorWindow>();
            window.CreateNewScript();
            window.Show();
        }

        public static void OpenAsset(ScriptData scriptData)
        {
            ScriptEditorWindow window = GetWindow<ScriptEditorWindow>();
            window.LoadScript(scriptData);
            window.Show();
        }

        public override string Title => "Script Editor";

        private ScriptView ScriptView => (ScriptView)View;

        private ScriptData _scriptData;

#region Unity Lifecycle
        protected override void OnEnable()
        {
            base.OnEnable();

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);

            CreateNodeView();
        }
#endregion

        protected override NodeEditorView CreateView()
        {
            return new ScriptView(this);
        }

        protected override IEdgeConnectorListener CreateEdgeConnectorListener()
        {
            return new EdgeConnectorListener(ScriptView);
        }

        private void CreateNewScript()
        {
            Debug.Log("Creating new script...");

            _scriptData = CreateInstance<ScriptData>();
            _scriptData.name = "ScriptData";
            ScriptView.LoadScript(_scriptData);
        }

        private void LoadScript(ScriptData scriptData)
        {
            Debug.Log($"Loading script {scriptData.name}...");

            _scriptData = scriptData;
            ScriptView.LoadScript(_scriptData);
        }
    }
}
