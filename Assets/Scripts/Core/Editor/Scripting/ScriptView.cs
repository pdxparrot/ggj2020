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
            // TODO: this isn't right, "ports" connect to "ports"
            // but we're only saving the node they connect to, not the "port"
            foreach(ScriptNodeData nodeData in _nodes.Values) {
                Type nodeType = nodeData.GetType();

                var connections = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => Attribute.IsDefined(x, typeof(ConnectionAttribute)));
                foreach(FieldInfo connection in connections) {
                    ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                    if(attr.ConnectionDirection != ConnectionAttribute.Direction.Output) {
                        continue;
                    }

                    ScriptNodeId outputId = (ScriptNodeId)connection.GetValue(nodeData);
                    if(outputId == 0) {
                        Debug.Log($"Node {nodeData.Id} connection {connection.Name} not connected");
                        continue;
                    }

                    ScriptNodeData outputNode = _nodes.GetOrDefault(outputId);
                    if(null == outputNode) {
                        Debug.LogWarning($"Node {nodeData.Id} connection {connection.Name} is connected to non-existent node {outputId}!");
                        continue;
                    }

                    Debug.LogWarning($"TODO: add connection edge from {nodeData.Id}:{connection.Name} to {outputNode.Id}");
                }

                var outputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(x => Attribute.IsDefined(x, typeof(OutputAttribute)));
                foreach(FieldInfo output in outputs) {
                    OutputAttribute attr = output.GetCustomAttribute<OutputAttribute>();

                    ScriptNodeId outputId = (ScriptNodeId)output.GetValue(nodeData);
                    if(outputId == 0) {
                        Debug.Log($"Node {nodeData.Id} output {output.Name} not connected");
                        continue;
                    }

                    ScriptNodeData outputNode = _nodes.GetOrDefault(outputId);
                    if(null == outputNode) {
                        Debug.LogWarning($"Node {nodeData.Id} output {output.Name} is connected to non-existent node {outputId}!");
                        continue;
                    }

                    Debug.LogWarning($"TODO: add output edge from {nodeData.Id}:{output.Name} to {outputNode.Id}");
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

        public void CreateNode(ScriptNodeData nodeData)
        {
            // prevent id conflicts
            while(_nodes.ContainsKey(nodeData.Id)) {
                nodeData.Id.GenerateId();
            }

            // convert the node screen position to the graph position
            Rect nodePosition = nodeData.Position;
            Vector2 windowMousePosition = Window.rootVisualElement.ChangeCoordinatesTo(Window.rootVisualElement.parent, nodePosition.position - Window.position.position);
            Vector2 graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
            nodeData.Position = new Rect(graphMousePosition, Vector2.zero);

            //Debug.Log($"Creating node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");
            _nodes[nodeData.Id] = nodeData;

            ScriptViewNode node = new ScriptViewNode(nodeData, Window.EdgeConnectorListener);
            AddElement(node);
        }

        public override List<Port> GetCompatiblePorts(Port startAnchor, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(x => x.portType == startAnchor.portType && x.direction != startAnchor.direction).ToList();
        }

#region Event Handlers
        private void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            CreateNodeWindow.ShowWindow(this, context.screenMousePosition);
        }
#endregion
    }
}
