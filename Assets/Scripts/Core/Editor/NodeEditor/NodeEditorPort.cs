using System;

using pdxpartyparrot.Core.Data.NodeEditor;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.NodeEditor
{
    public abstract class NodeEditorPort : Port
    {
        public Guid Id => PortData.Id;

        public NodeEditorNode Node { get; }

        public NodePortData PortData { get; }

        public NodeEditorPort(NodeEditorNode node, NodePortData portData, Orientation orientation, Direction direction, Capacity capacity, Type type) : base(orientation, direction, capacity, type)
        {
            Node = node;
            PortData = portData;
        }
    }
}
