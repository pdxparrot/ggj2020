using System;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.NodeEditor;

using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptViewNode : NodeEditorNode
    {
        protected override string MainStyleSheet => "ScriptEditorWindow/Node";

        protected override string NodeLayout => "ScriptEditorWindow/Node";

        private ScriptNodeData ScriptNodeData => (ScriptNodeData)NodeData;

        public ScriptViewNode(ScriptNodeData nodeData, IEdgeConnectorListener edgeConnectorListener) : base(nodeData, edgeConnectorListener)
        {
        }

        protected override void InitializeView()
        {
            base.InitializeView();

            Type nodeType = ScriptNodeData.GetType();
            //Debug.Log($"Node 0x{Id:X} of type {nodeType}");

            var connections = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(ConnectionAttribute)));
            foreach(FieldInfo connection in connections) {
                ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                if(attr.ConnectionDirection != ConnectionAttribute.Direction.Input) {
                    continue;
                }
                //Debug.Log($"Add input connection {attr.Name} to node 0x{Id:X}");

                ScriptNodePortData inputPort = (ScriptNodePortData)connection.GetValue(ScriptNodeData);
                ScriptViewPort port = new ScriptViewPort(this, inputPort, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(ScriptNodePortData))
                {
                    portName = attr.Name
                };
                port.AddManipulator(new EdgeConnector<Edge>(EdgeConnectorListener));
                Add(port);
            }

            var inputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(InputAttribute)));
            foreach(FieldInfo input in inputs) {
                InputAttribute attr = input.GetCustomAttribute<InputAttribute>();
                //Debug.Log($"Add input {attr.Name} of type {attr.Type} to node 0x{Id:X}");

                ScriptNodePortData inputPort = (ScriptNodePortData)input.GetValue(ScriptNodeData);
                ScriptViewPort port = new ScriptViewPort(this, inputPort, Orientation.Horizontal, Direction.Input, Port.Capacity.Single, attr.Type)
                {
                    portName = attr.Name
                };
                port.AddManipulator(new EdgeConnector<Edge>(EdgeConnectorListener));
                Add(port);
            }

            foreach(FieldInfo connection in connections) {
                ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                if(attr.ConnectionDirection != ConnectionAttribute.Direction.Output) {
                    continue;
                }
                //Debug.Log($"Add output connection {attr.Name} to node 0x{Id:X}");

                ScriptNodePortData outputPort = (ScriptNodePortData)connection.GetValue(ScriptNodeData);
                ScriptViewPort port = new ScriptViewPort(this, outputPort, Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(ScriptNodePortData))
                {
                    portName = attr.Name
                };
                port.AddManipulator(new EdgeConnector<Edge>(EdgeConnectorListener));
                Add(port);
            }

            var outputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(OutputAttribute)));
            foreach(FieldInfo output in outputs) {
                OutputAttribute attr = output.GetCustomAttribute<OutputAttribute>();
                //Debug.Log($"Add output {attr.Name} of type {attr.Type} to node 0x{Id:X}");

                ScriptNodePortData outputPort = (ScriptNodePortData)output.GetValue(ScriptNodeData);
                ScriptViewPort port = new ScriptViewPort(this, outputPort, Orientation.Horizontal, Direction.Output, Port.Capacity.Single, attr.Type)
                {
                    portName = attr.Name
                };
                port.AddManipulator(new EdgeConnector<Edge>(EdgeConnectorListener));
                Add(port);
            }
        }
    }
}
