using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

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

        private static void Show(ScriptView scriptView, Vector2 nodePosition, Action<ScriptViewNode> onSuccess)
        {
            CreateNodeWindow window = GetWindow<CreateNodeWindow>();
            window._nodePosition = nodePosition;
            window._onSuccess = onSuccess;
            window.ScriptView = scriptView;
            window.Show();
        }

        public static void ShowForCreate(ScriptView scriptView, Vector2 nodePosition)
        {
            Show(scriptView, nodePosition, null);
        }

        public static void ShowForDrop(ScriptView scriptView, Vector2 nodePosition, Action<ScriptViewNode> onSuccess)
        {
            Show(scriptView, nodePosition, onSuccess);
        }

        public override string Title => "Add Scripting Node";

        private readonly List<Type> _nodeTypes = new List<Type>();
        private readonly Dictionary<string, Type> _nodes = new Dictionary<string, Type>();
        private readonly List<string> _filteredNodeNames = new List<string>();

        private TextField _filter;

        private ListView _nodeList;

        private Vector2 _nodePosition;

        [CanBeNull]
        private Action<ScriptViewNode> _onSuccess;

        private ScriptView _scriptView;

        public ScriptView ScriptView
        {
            get => _scriptView;

            private set
            {
                _scriptView = value;

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

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);

            _filter = rootVisualElement.Q<TextField>("node-filter");
            _filter.RegisterValueChangedCallback(FilterChangedEventHandler);

            _nodeList = rootVisualElement.Q<ListView>("node-list");
            _nodeList.makeItem = MakeItemEventHandler;
            _nodeList.bindItem = BindItemEventHandler;
            _nodeList.itemsSource = _filteredNodeNames;
            _nodeList.selectionType = SelectionType.Single;
            _nodeList.onItemChosen += ItemChosenEventHandler;
        }
#endregion

        // TODO: this could be done much better
        private void Filter()
        {
            _filteredNodeNames.Clear();

            _filteredNodeNames.AddRange(_nodes.Keys.Where(x => {
                if(string.IsNullOrWhiteSpace(x)) {
                    return false;
                }

                Type nodeType = _nodes[x];

                ScriptNodeAttribute attr = nodeType.GetCustomAttribute<ScriptNodeAttribute>();
                if(null == attr || (!attr.AllowMultiple && null != _scriptView && _scriptView.ScriptData.Nodes.Any(y => y.GetType() == nodeType))) {
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

            ScriptNodeData nodeData = (ScriptNodeData)Activator.CreateInstance(_nodes[nodeName], new Rect(_nodePosition, Vector2.zero));
            ScriptViewNode node = ScriptView.CreateNode(nodeData, null == _onSuccess);

            _onSuccess?.Invoke(node);

            Close();
        }
#endregion
    }
}
