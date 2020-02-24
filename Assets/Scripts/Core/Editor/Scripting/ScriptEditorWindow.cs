using pdxpartyparrot.Core.Data.Scripting;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    // https://forum.unity.com/threads/bare-bones-graphview-example.778706/
    // https://forum.unity.com/threads/how-to-use-the-new-graphview-uielements.536563/
    public sealed class ScriptEditorWindow : Window.EditorWindow
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

        private ScriptView _scriptView;

        private ScriptData _scriptData;

#region Unity Lifecycle
        protected override void OnEnable()
        {
            base.OnEnable();

            VisualRoot.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(VisualRoot);

            CreateScriptView();
        }
#endregion

        private void CreateNewScript()
        {
            Debug.Log("Creating new script...");

            _scriptData = CreateInstance<ScriptData>();
            _scriptData.name = "ScriptData";

            _scriptView.LoadScript(_scriptData);
        }

        private void LoadScript(ScriptData scriptData)
        {
            Debug.Log($"Loading script {scriptData.name}...");

            _scriptData = scriptData;

            _scriptView.LoadScript(_scriptData);
        }

        private void CreateScriptView()
        {
            Assert.IsNull(_scriptView);
            _scriptView = new ScriptView();

            VisualRoot.Clear();
            VisualRoot.Add(_scriptView);

            _scriptView.AddManipulator(new ContentDragger());
            _scriptView.AddManipulator(new SelectionDragger());
            _scriptView.AddManipulator(new RectangleSelector());
            _scriptView.AddManipulator(new ClickSelector());
            _scriptView.graphViewChanged = ScriptViewChanged;

            _scriptView.StretchToParentSize();
        }

#region Event Handlers
        private GraphViewChange ScriptViewChanged(GraphViewChange graphViewChange)
        {
            // TODO: save if we can?

            return graphViewChange;
        }
#endregion
    }
}
