using System;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.Window;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptViewNode : Node
    {
        private const string MainStyleSheet = "ScriptEditorWindow/Node";
        private const string NodeLayout = "ScriptEditorWindow/Node";

        public int Id => _nodeData.Id;

        private IEdgeConnectorListener _edgeConnectorListener;

        private ScriptNodeData _nodeData;

        public ScriptViewNode(ScriptNodeData nodeData, IEdgeConnectorListener edgeConnectorListener) : base()
        {
            _nodeData = nodeData;
            _edgeConnectorListener = edgeConnectorListener;

            styleSheets.Add(Resources.Load<StyleSheet>(EditorWindow.CoreStyleSheet));
            styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(NodeLayout);
            mainVisualTree.CloneTree(this);

            title = _nodeData.Name;

            InitializeView();

            SetPosition(_nodeData.Position);
        }

        private void InitializeView()
        {
            Label label = new Label
            {
                text = $"ID: 0x{Id:X}"
            };
            Add(label);

            Type nodeType = _nodeData.GetType();
            //Debug.Log($"Node 0x{Id:X} of type {nodeType}");

            var connections = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(ConnectionAttribute)));
            foreach(FieldInfo connection in connections) {
                ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                if(attr.ConnectionDirection != ConnectionAttribute.Direction.Input) {
                    continue;
                }
                //Debug.Log($"Add input connection {attr.Name} of type {connection.FieldType} to node 0x{Id:X}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, connection.FieldType);
                port.portName = attr.Name;
                port.AddManipulator(new EdgeConnector<Edge>(_edgeConnectorListener));
                Add(port);
            }

            var inputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(InputAttribute)));
            foreach(FieldInfo input in inputs) {
                InputAttribute attr = input.GetCustomAttribute<InputAttribute>();
                //Debug.Log($"Add input {attr.Name} of type {input.FieldType} to node 0x{Id:X}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, input.FieldType);
                port.portName = attr.Name;
                port.AddManipulator(new EdgeConnector<Edge>(_edgeConnectorListener));
                Add(port);
            }

            foreach(FieldInfo connection in connections) {
                ConnectionAttribute attr = connection.GetCustomAttribute<ConnectionAttribute>();
                if(attr.ConnectionDirection != ConnectionAttribute.Direction.Output) {
                    continue;
                }
                //Debug.Log($"Add output connection {attr.Name} of type {connection.FieldType} to node 0x{Id:X}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, connection.FieldType);
                port.portName = attr.Name;
                port.AddManipulator(new EdgeConnector<Edge>(_edgeConnectorListener));
                Add(port);
            }

            var outputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(OutputAttribute)));
            foreach(FieldInfo output in outputs) {
                OutputAttribute attr = output.GetCustomAttribute<OutputAttribute>();
                //Debug.Log($"Add output {attr.Name} of type {output.FieldType} to node 0x{Id:X}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, output.FieldType);
                port.portName = attr.Name;
                port.AddManipulator(new EdgeConnector<Edge>(_edgeConnectorListener));
                Add(port);
            }
        }
    }
}
