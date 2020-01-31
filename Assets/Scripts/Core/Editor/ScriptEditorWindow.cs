using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor
{
    public sealed class ScriptEditorWindow : Window.EditorWindow
    {
        private const string MainStyleSheet = "ScriptEditorWindow/Main";
        private const string WindowLayout = "ScriptEditorWindow/Window";

        [MenuItem("PDX Party Parrot/Core/Script Editor...")]
        static void ShowWindow()
        {
            ScriptEditorWindow window = GetWindow<ScriptEditorWindow>();
            window.Show();
        }

        public override string Title => "Script Editor";

        private Vector2 _xScrollPosition, _yScrollPosition;

#region Unity Lifecycle
        protected override void OnEnable()
        {
            base.OnEnable();

            VisualRoot.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(VisualRoot);
        }
#endregion
    }
}
