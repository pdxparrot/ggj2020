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

        public Guid Id => _nodeData.Id;

        private ScriptNodeData _nodeData;

        public ScriptViewNode(ScriptNodeData nodeData) : base()
        {
            _nodeData = nodeData;

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
                text = $"ID: {Id}"
            };
            Add(label);

            Type nodeType = _nodeData.GetType();
            Debug.Log($"Node type {nodeType}");

            var inputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(InputAttribute)));
            foreach(FieldInfo input in inputs) {
                InputAttribute attr = (InputAttribute)input.GetCustomAttribute(typeof(InputAttribute));
                //Debug.Log($"Add input {attr.Name} to node {Id}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, input.GetType());
                port.portName = attr.Name;
                Add(port);
            }

            var outputs = nodeType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(x => Attribute.IsDefined(x, typeof(OutputAttribute)));
            foreach(FieldInfo output in outputs) {
                OutputAttribute attr = (OutputAttribute)output.GetCustomAttribute(typeof(OutputAttribute));
                //Debug.Log($"Add output {attr.Name} to node {Id}");

                Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, output.GetType());
                port.portName = attr.Name;
                Add(port);
            }
        }
    }
}
