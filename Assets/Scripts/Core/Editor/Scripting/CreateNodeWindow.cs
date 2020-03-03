using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.Scripting;
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

        public static void ShowWindow(ScriptData scriptData)
        {
            CreateNodeWindow window = GetWindow<CreateNodeWindow>();
            window.ScriptData = scriptData;
            window.Show();
        }

        public override string Title => "Add Scripting Node";

        private readonly List<Type> _nodeTypes = new List<Type>();
        private readonly Dictionary<string, Type> _nodes = new Dictionary<string, Type>();
        private readonly List<string> _filteredNodeNames = new List<string>();

        private TextField _filter;

        private ListView _nodeList;

        private ScriptData _scriptData;

        public ScriptData ScriptData
        {
            get => _scriptData;

            private set
            {
                _scriptData = value;

                Filter();
            }
        }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            ReflectionUtils.FindSubClassesOfInNamespace<ScriptNodeData>(_nodeTypes, EditorSettings.projectGenerationRootNamespace);
            foreach(Type nodeType in _nodeTypes) {
                ScriptNodeAttribute attr = nodeType.GetCustomAttribute<ScriptNodeAttribute>();
                if(null == attr) {
                    Debug.LogWarning($"Node type {nodeType} missing node attribute!");
                    continue;
                }

                _nodes.Add(attr.Name, nodeType);
            }
            //Debug.Log($"Found {_nodes.Count} nodes");
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

        // TODO: this could be done much better
        private void Filter()
        {
            _filteredNodeNames.Clear();

            _filteredNodeNames.AddRange(_nodes.Keys.Where(x => {
                if(null == x) {
                    return false;
                }

                Type nodeType = _nodes[x];

                ScriptNodeAttribute attr = nodeType.GetCustomAttribute<ScriptNodeAttribute>();
                if(null == attr || (!attr.AllowMultiple && null != _scriptData && _scriptData.Nodes.Any(y => y.GetType() == nodeType))) {
                    return false;
                }

                return -1 != x.IndexOf(_filter.text, StringComparison.InvariantCultureIgnoreCase);
            }));

            _nodeList.itemsSource = _filteredNodeNames;
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
            string nodeName = item as string;

            Debug.Log($"selected node {nodeName} of type {_nodes[nodeName]}");
        }
#endregion
    }
}
