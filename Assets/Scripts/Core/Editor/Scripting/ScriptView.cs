using System.Collections.Generic;
using System.Linq;

using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : GraphView
    {
        private ScriptEditorWindow _window;

        public ScriptData ScriptData { get; private set; }

        public ScriptView(ScriptEditorWindow window) : base()
        {
            _window = window;

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

            foreach(ScriptNodeData nodeData in scriptData.Nodes) {
                AddNode(nodeData);
            }

            // TODO: add edges
        }

        public void AddNode(ScriptNodeData nodeData)
        {
            //Debug.Log($"Adding node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");

            ScriptViewNode node = new ScriptViewNode(nodeData, _window.EdgeConnectorListener);
            AddElement(node);
        }

        private void AddEdge()
        {
            // TODO
        }

        public void CreateNode(ScriptNodeData nodeData)
        {
            // convert the node screen position to the graph position
            Rect nodePosition = nodeData.Position;
            Vector2 windowMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent, nodePosition.position - _window.position.position);
            Vector2 graphMousePosition = contentViewContainer.WorldToLocal(windowMousePosition);
            nodeData.Position = new Rect(graphMousePosition, Vector2.zero);

            //Debug.Log($"Creating node {nodeData.Id} of type {nodeData.GetType()} at {nodeData.Position}");

            ScriptViewNode node = new ScriptViewNode(nodeData, _window.EdgeConnectorListener);
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
