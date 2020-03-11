using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Editor.Window;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.NodeEditor
{
    public abstract class NodeEditorNode : Node
    {
        protected abstract string MainStyleSheet { get; }

        protected abstract string NodeLayout { get; }

        public NodeId Id => NodeData.Id;

        public Rect Position
        {
            get => NodeData.Position;
            set => NodeData.Position = value;
        }

        protected NodeData NodeData { get; private set; }

        protected IEdgeConnectorListener EdgeConnectorListener { get; private set; }

        public NodeEditorNode(NodeData nodeData, IEdgeConnectorListener edgeConnectorListener) : base()
        {
            NodeData = nodeData;
            EdgeConnectorListener = edgeConnectorListener;

            styleSheets.Add(Resources.Load<StyleSheet>(EditorWindow.CoreStyleSheet));
            styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(NodeLayout);
            mainVisualTree.CloneTree(this);

            title = NodeData.Title;

            InitializeView();

            SetPosition(NodeData.Position);
        }

        protected virtual void InitializeView()
        {
            Label label = new Label
            {
                text = $"ID: 0x{(int)Id:X}"
            };
            Add(label);
        }
    }
}
