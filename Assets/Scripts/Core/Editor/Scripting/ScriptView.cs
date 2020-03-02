using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : GraphView
    {
        private ScriptData _scriptData;

        public ScriptView() : base()
        {
            GridBackground gridBackground = new GridBackground();
            Add(gridBackground);
            gridBackground.SendToBack();

            nodeCreationRequest = NodeCreationRequestEventHandler;
        }

        public void LoadScript(ScriptData scriptData)
        {
            _scriptData = scriptData;

            DeleteElements(graphElements.ToList());

            //Debug.Log($"Loading script '{scriptData.name}' with {scriptData.Nodes.Count} nodes...");

            foreach(IScriptNodeData nodeData in scriptData.Nodes) {
                AddNode(nodeData);
            }
        }

        public void AddNode(IScriptNodeData nodeData)
        {
            //Debug.Log($"Adding node {nodeData.Id} of type {nodeData.GetType()}");

            ScriptViewNode node = new ScriptViewNode(nodeData);
            AddElement(node);
        }

#region Event Handlers
        private void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            CreateNodeWindow.ShowWindow(_scriptData);
        }
#endregion
    }
}
