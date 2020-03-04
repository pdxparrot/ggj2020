using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor
{
    public sealed class BuildWindow : Window.EditorWindow
    {
        private const string MainStyleSheet = "BuildWindow/Main";
        private const string WindowLayout = "BuildWindow/Window";

        [MenuItem("PDX Party Parrot/Build...")]
        public static void ShowWindow()
        {
            BuildWindow window = GetWindow<BuildWindow>();
            window.Show();
        }

        public override string Title => "Build";

#region Unity Lifecycle
        protected override void OnEnable()
        {
            base.OnEnable();

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);
        }
#endregion
    }
}
