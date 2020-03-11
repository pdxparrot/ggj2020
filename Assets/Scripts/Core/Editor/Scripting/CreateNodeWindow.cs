using System;
using System.Collections;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.NodeEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class CreateNodeWindow : CreateNodeWindow<ScriptNodeData, ScriptNodeAttribute, ScriptViewNode>
    {
        private const string MainStyleSheet = "ScriptEditorWindow/CreateNodeWindow/Main";
        private const string WindowLayout = "ScriptEditorWindow/CreateNodeWindow/Window";

        private static void Show(ScriptView scriptView, Vector2 nodePosition, Action<ScriptViewNode> onSuccess)
        {
            CreateNodeWindow window = GetWindow<CreateNodeWindow>();
            window._nodePosition = nodePosition;
            window._onSuccess = onSuccess;
            window.View = scriptView;
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

        private Vector2 _nodePosition;

        [CanBeNull]
        private Action<ScriptViewNode> _onSuccess;

        private TextField _filter;

        private ListView _nodeList;

        private ScriptView ScriptView => (ScriptView)View;

#region Unity Lifecycle
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
            _nodeList.itemsSource = null;
            _nodeList.selectionType = SelectionType.Single;
            _nodeList.onItemChosen += ItemChosenEventHandler;
        }
#endregion

        protected override bool FilterNode(Type nodeType, ScriptNodeAttribute attr)
        {
            if(!attr.AllowMultiple && null != ScriptView && ScriptView.ScriptData.Nodes.Any(y => y.GetType() == nodeType)) {
                return false;
            }
            return true;
        }

        protected override void SetNodeList(IList nodes)
        {
            _nodeList.itemsSource = nodes;
        }

        protected override ScriptViewNode CreateViewNode(ScriptNodeData nodeData)
        {
            return (ScriptViewNode)ScriptView.CreateNode(nodeData, null == _onSuccess);
        }

#region Event Handlers
        private void FilterChangedEventHandler(ChangeEvent<string> evt)
        {
            FilterNodes(_filter.text);
        }

        private VisualElement MakeItemEventHandler()
        {
            return new Label();
        }

        private void BindItemEventHandler(VisualElement element, int index)
        {
            (element as Label).text = GetNodeName(index);
        }

        private void ItemChosenEventHandler(object item)
        {
            string nodeName = item as string;

            ScriptViewNode node = CreateNode(nodeName, _nodePosition);

            _onSuccess?.Invoke(node);

            Close();
        }
#endregion
    }
}
