using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : GraphView
    {
        public ScriptData ScriptData { get; private set; }

        public ScriptView() : base()
        {
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
        }

        public void AddNode(ScriptNodeData nodeData)
        {
            //Debug.Log($"Adding node {nodeData.Id} of type {nodeData.GetType()}");

            ScriptViewNode node = new ScriptViewNode(nodeData);
            AddElement(node);
        }

#region Event Handlers
        private void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            CreateNodeWindow.ShowWindow(this, context.screenMousePosition);
        }
#endregion
    }
}
