using System;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.NodeEditor;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : NodeEditorView
    {
        public ScriptData ScriptData { get; private set; }

        public ScriptView(ScriptEditorWindow window) : base(window)
        {
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
            foreach(ScriptNodeData nodeData in Nodes.Values) {
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

        protected override NodeEditorNode CreateNode(NodeData nodeData)
        {
            return new ScriptViewNode((ScriptNodeData)nodeData, Window.EdgeConnectorListener);
        }

#region Event Handlers
        protected override void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            CreateNodeWindow.ShowForCreate(this, context.screenMousePosition);
        }
#endregion
    }
}
