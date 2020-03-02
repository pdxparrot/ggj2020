using System;
using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.Core.Data.Scripting.Nodes;
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
        private readonly List<string> _filteredNodeNames = new List<string>();

        private TextField _filter;

        private ListView _nodeList;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            ReflectionUtils.FindSubClassesOfInNamespace<ScriptNodeData>(_nodeTypes, EditorSettings.projectGenerationRootNamespace);
            foreach(Type t in _nodeTypes) {
                // TODO: this should be the name property of the node somehow
                // might need to use a new attribute for that
                _nodeNames.Add(t.Namespace?.StartsWith(EditorSettings.projectGenerationRootNamespace) ?? false ? t.FullName : t.Name);
            }
            //Debug.Log($"Found {_nodeNames.Count} nodes");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            VisualRoot.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(VisualRoot);

            _filter = VisualRoot.Q<TextField>("node-filter");
            _filter.RegisterValueChangedCallback(FilterChangedEventHandler);

            _nodeList = VisualRoot.Q<ListView>("node-list");
            _nodeList.makeItem = MakeItemEventHandler;
            _nodeList.bindItem = BindItemEventHandler;
            _nodeList.itemsSource = _filteredNodeNames;
            _nodeList.selectionType = SelectionType.Single;
            _nodeList.onItemChosen += ItemChosenEventHandler;

            Filter();
        }

        protected override void OnDisable()
        {
            _filter.UnregisterCallback<ChangeEvent<string>>(FilterChangedEventHandler);
            _filter = null;

            _nodeList.onItemChosen -= ItemChosenEventHandler;
            _nodeList.itemsSource = null;
            _nodeList.bindItem = null;
            _nodeList.makeItem = null;
            _nodeList = null;

            base.OnDisable();
        }
#endregion

        private void Filter()
        {
            _filteredNodeNames.Clear();

            _filteredNodeNames.AddRange(_nodeNames.Where(x => x != null && x.Contains(_filter.text)));

            _nodeList.MarkDirtyRepaint();
        }

#region Event Handlers
        private void FilterChangedEventHandler(ChangeEvent<string> evt)
        {
            Filter();
        }

        private VisualElement MakeItemEventHandler()
        {
            return new Label();
        }

        private void BindItemEventHandler(VisualElement element, int index)
        {
            (element as Label).text = _filteredNodeNames[index];
        }

        private void ItemChosenEventHandler(object item)
        {
            Debug.Log($"selected node {item}");
        }
#endregion
    }
}
