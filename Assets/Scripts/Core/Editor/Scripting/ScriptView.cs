using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Collections;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : GraphView
    {
        public ScriptEditorWindow Window { get; private set; }

        public ScriptData ScriptData { get; private set; }

        private readonly Dictionary<ScriptNodeId, ScriptNodeData> _nodes = new Dictionary<ScriptNodeId, ScriptNodeData>();

        public ScriptView(ScriptEditorWindow window) : base()
        {
            Window = window;

            GridBackground gridBackground = new GridBackground();
            Add(gridBackground);
            gridBackground.SendToBack();

            nodeCreationRequest = NodeCreationRequestEventHandler;
        }

        public void LoadScript(ScriptData scriptData)
        {
            ScriptData = scriptData;

            DeleteElements(graphElements.ToList());

            //Debug.Log($"Loading script '{scriptData.name}' with {scriptData.Nodes.Count} nodes...");

            // add nodes
            foreach(ScriptNodeData nodeData in scriptData.Nodes) {
                AddNode(nodeData);
            }

            // add edges
            foreach(ScriptNodeData nodeData in _nodes.Values) {
                Type nodeType = nodeData.GetType();

                var connections = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => Attribute.IsDefined(x, typeof(ConnectionAttribute)));
                foreach(FieldInfo connection in connections) {
                    ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                    if(attr.ConnectionDirection != ConnectionAttribute.Direction.Output) {
                        continue;
                    }

                    ScriptNodePortData outputPort = (ScriptNodePortData)connection.GetValue(nodeData);
                    if(!outputPort.IsConnected) {
                        //Debug.Log($"Node {nodeData.Id} connection {outputPort.Id} not connected");
                        continue;
                    }

                    AddEdge(nodeData.Id, outputPort.Id, outputPort.NodeId, outputPort.PortId);
                }

                var outputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => Attribute.IsDefined(x, typeof(OutputAttribute)));
                foreach(FieldInfo output in outputs) {
                    OutputAttribute attr = output.GetCustomAttribute<OutputAttribute>();

                    ScriptNodePortData outputPort = (ScriptNodePortData)output.GetValue(nodeData);
                    if(!outputPort.IsConnected) {
                        //Debug.Log($"Node {nodeData.Id} output {outputPort.Id} not connected");
                        continue;
                    }

                    AddEdge(nodeData.Id, outputPort.Id, outputPort.NodeId, outputPort.PortId);
                }
            }
        }

        private void AddNode(ScriptNodeData nodeData)
        {
            Assert.IsFalse(_nodes.ContainsKey(nodeData.Id));

            //Debug.Log($"Adding node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");
            _nodes[nodeData.Id] = nodeData;

            ScriptViewNode node = new ScriptViewNode(nodeData, Window.EdgeConnectorListener);
            AddElement(node);
        }

        public ScriptViewNode CreateNode(ScriptNodeData nodeData, bool isAtScreenPosition)
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

            ScriptViewNode node = new ScriptViewNode(nodeData, Window.EdgeConnectorListener);
            AddElement(node);

            return node;
        }

        public void AddEdge(ScriptNodeId outputNodeId, Guid outputPortId, ScriptNodeId inputNodeId, Guid inputPortId)
        {
            Debug.LogWarning($"TODO: add edge from 0x{(int)outputNodeId:X}:{outputPortId} to 0x{(int)inputNodeId:X}:{inputPortId}");

            ScriptNodeData outputNode = _nodes.GetOrDefault(outputNodeId);
            if(null == outputNode) {
                Debug.LogWarning($"Invalid edge output node {outputNodeId:X}");
                return;
            }

            ScriptNodeData inputNode = _nodes.GetOrDefault(inputNodeId);
            if(null == inputNode) {
                Debug.LogWarning($"Invalid edge input node {inputNodeId:X}");
                return;
            }
        }

        public void CreateEdge(ScriptNodeId outputNodeId, Guid outputPortId, ScriptNodeId inputNodeId, Guid inputPortId)
        {
            Debug.LogWarning($"TODO: create edge from 0x{(int)outputNodeId:X}:{outputPortId} to 0x{(int)inputNodeId:X}:{inputPortId}");

            ScriptNodeData outputNode = _nodes.GetOrDefault(outputNodeId);
            if(null == outputNode) {
                Debug.LogWarning($"Invalid edge output node {outputNodeId:X}");
                return;
            }

            ScriptNodeData inputNode = _nodes.GetOrDefault(inputNodeId);
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
        private void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            CreateNodeWindow.ShowForCreate(this, context.screenMousePosition);
        }
#endregion
    }
}
