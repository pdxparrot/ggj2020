using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Project
{
    public class ProjectSettingsWindow : Window.EditorWindow
    {
        private const string MainStyleSheet = "ProjectSettingsWindow/Main";
        private const string WindowLayout = "ProjectSettingsWindow/Window";

        [MenuItem("PDX Party Parrot/Project Settings...")]
        public static void ShowWindow()
        {
            ProjectSettingsWindow window = GetWindow<ProjectSettingsWindow>();
            window.Show();
        }

        public override string Title => "Project Settings";

        private EnumField _behaviorMode;

        private TextField _productName;
        private TextField _productVersion;

        private Toggle _useSpine;
        private Toggle _useDOTween;
        private Toggle _useNetworking;

#region Unity Lifecycle
        protected override void OnDestroy()
        {
            OnSave();

            base.OnDestroy();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);

            ProjectManifest manifest = new ProjectManifest();
            manifest.Read();

            _behaviorMode = rootVisualElement.Q<EnumField>("enum-behavior-mode");
            _behaviorMode.Init(EditorBehaviorMode.Mode3D);
            _behaviorMode.value = EditorSettings.defaultBehaviorMode;

            _productName = rootVisualElement.Q<TextField>("text-product-name");
            _productName.value = PlayerSettings.productName;

            _productVersion = rootVisualElement.Q<TextField>("text-product-version");
            _productVersion.value = PlayerSettings.bundleVersion;

            _useSpine = rootVisualElement.Q<Toggle>("toggle-feature-spine");
            _useSpine.value = manifest.UseSpine;

            _useDOTween = rootVisualElement.Q<Toggle>("toggle-feature-dotween");
            _useDOTween.value = manifest.UseDOTween;

            _useNetworking = rootVisualElement.Q<Toggle>("toggle-feature-networking");
            _useNetworking.value = manifest.UseNetworking;
        }
#endregion

        private void SetScriptingDefineSymbols(BuildTargetGroup targetGroup)
        {
            ScriptingDefineSymbols scriptingDefineSymbols = new ScriptingDefineSymbols(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup));

            if(_useSpine.value) {
                scriptingDefineSymbols.AddSymbol("USE_SPINE");
            } else {
                scriptingDefineSymbols.RemoveSymbol("USE_SPINE");
            }

            if(_useDOTween.value) {
                scriptingDefineSymbols.AddSymbol("USE_DOTWEEN");
            } else {
                scriptingDefineSymbols.RemoveSymbol("USE_DOTWEEN");
            }

            if(_useNetworking.value) {
                scriptingDefineSymbols.AddSymbol("USE_NETWORKING");
            } else {
                scriptingDefineSymbols.RemoveSymbol("USE_NETWORKING");
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, scriptingDefineSymbols.ToString());
        }

#region Events
        private void OnSave()
        {
            ProjectManifest manifest = new ProjectManifest();
            manifest.Read();

            bool refreshAssetDatabase = false;

            EditorSettings.defaultBehaviorMode = (EditorBehaviorMode)_behaviorMode.value;

            PlayerSettings.productName = _productName.value;
            PlayerSettings.bundleVersion = _productVersion.value;

            refreshAssetDatabase |= manifest.UseSpine != _useSpine.value;
            manifest.UseSpine = _useSpine.value;

            refreshAssetDatabase |= manifest.UseDOTween != _useDOTween.value;
            manifest.UseDOTween = _useDOTween.value;

            refreshAssetDatabase |= manifest.UseNetworking != _useNetworking.value;
            manifest.UseNetworking = _useNetworking.value;

            SetScriptingDefineSymbols(BuildTargetGroup.Standalone);
            SetScriptingDefineSymbols(BuildTargetGroup.Android);
            SetScriptingDefineSymbols(BuildTargetGroup.iOS);
            SetScriptingDefineSymbols(BuildTargetGroup.WebGL);

            manifest.Write();

            if(refreshAssetDatabase) {
                AssetDatabase.Refresh();
            }
        }
#endregion
    }
}
