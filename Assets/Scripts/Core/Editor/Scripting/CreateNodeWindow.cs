using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class CreateNodeWindow : Window.EditorWindow
    {
        private const string MainStyleSheet = "ScriptEditorWindow/CreateNodeWindow/Main";
        private const string WindowLayout = "ScriptEditorWindow/CreateNodeWindow/Window";

        public static void ShowWindow()
        {
            CreateNodeWindow window = GetWindow<CreateNodeWindow>();
            window.Show();
        }

        public override string Title => "Add Scripting Node";

        private readonly List<Type> _nodeTypes = new List<Type>();
        private readonly List<string> _nodeNames = new List<string>();


#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _nodeTypes.Add(null);
            ReflectionUtils.FindSubClassesOfInNamespace<Component>(_nodeTypes, EditorSettings.projectGenerationRootNamespace);

            foreach(Type t in _nodeTypes) {
                if(null == t) {
                    _nodeNames.Add("None");
                    continue;
                }
                _nodeNames.Add(t.Namespace?.StartsWith(EditorSettings.projectGenerationRootNamespace) ?? false ? t.FullName : t.Name);
            }
        }

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
