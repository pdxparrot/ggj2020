using System;
using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data.NodeEditor;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.NodeEditor
{
    public abstract class NodeEditorView : GraphView
    {
        public NodeEditorWindow Window { get; private set; }

        private readonly Dictionary<NodeId, NodeData> _nodes = new Dictionary<NodeId, NodeData>();

        protected IReadOnlyDictionary<NodeId, NodeData> Nodes => _nodes;

        public NodeEditorView(NodeEditorWindow window) : base()
        {
            Window = window;

            GridBackground gridBackground = new GridBackground();
            Add(gridBackground);
            gridBackground.SendToBack();

            nodeCreationRequest = NodeCreationRequestEventHandler;
        }

        protected abstract NodeEditorNode CreateNode(NodeData nodeData);

        protected virtual void AddNode(NodeData nodeData)
        {
            Assert.IsFalse(_nodes.ContainsKey(nodeData.Id));

            //Debug.Log($"Adding node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");
            _nodes[nodeData.Id] = nodeData;

            NodeEditorNode node = CreateNode(nodeData);
            AddElement(node);
        }

        public NodeEditorNode CreateNode(NodeData nodeData, bool isAtScreenPosition)
        {
            // prevent id conflicts
            while(_nodes.ContainsKey(nodeData.Id)) {
                nodeData.Id.GenerateId();
            }

            // convert the node screen position to the graph position
            if(isAtScreenPosition) {
                Rect nodePosition = nodeData.Position;
                Vector2 windowMousePosition = Window.rootVisualElement.ChangeCoordinatesTo(Window.rootVisualElement.parent, nodePosition.position - Window.position.position);
                Vector2 graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
                nodeData.Position = new Rect(graphMousePosition, Vector2.zero);
            }

            //Debug.Log($"Creating node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");
            _nodes[nodeData.Id] = nodeData;

            NodeEditorNode node = CreateNode(nodeData);
            AddElement(node);

            return node;
        }

        public void AddEdge(NodeId outputNodeId, Guid outputPortId, NodeId inputNodeId, Guid inputPortId)
        {
            Debug.LogWarning($"TODO: add edge from 0x{(int)outputNodeId:X}:{outputPortId} to 0x{(int)inputNodeId:X}:{inputPortId}");

            NodeData outputNode = _nodes.GetOrDefault(outputNodeId);
            if(null == outputNode) {
                Debug.LogWarning($"Invalid edge output node {outputNodeId:X}");
                return;
            }

            NodeData inputNode = _nodes.GetOrDefault(inputNodeId);
            if(null == inputNode) {
                Debug.LogWarning($"Invalid edge input node {inputNodeId:X}");
                return;
            }
        }

        public void CreateEdge(NodeId outputNodeId, Guid outputPortId, NodeId inputNodeId, Guid inputPortId)
        {
            Debug.LogWarning($"TODO: create edge from 0x{(int)outputNodeId:X}:{outputPortId} to 0x{(int)inputNodeId:X}:{inputPortId}");

            NodeData outputNode = _nodes.GetOrDefault(outputNodeId);
            if(null == outputNode) {
                Debug.LogWarning($"Invalid edge output node {outputNodeId:X}");
                return;
            }

            NodeData inputNode = _nodes.GetOrDefault(inputNodeId);
            if(null == inputNode) {
                Debug.LogWarning($"Invalid edge input node {inputNodeId:X}");
                return;
            }
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(x => x.portType == startAnchor.portType && x.direction != startAnchor.direction).ToList();
        }

#region Event Handlers
        protected abstract void NodeCreationRequestEventHandler(NodeCreationContext context);
#endregion
    }
}
