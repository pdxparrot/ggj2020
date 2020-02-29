using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptView : GraphView
    {
        public ScriptView() : base()
        {
            GridBackground gridBackground = new GridBackground();
            Add(gridBackground);
            gridBackground.SendToBack();
        }

        public void LoadScript(ScriptData scriptData)
        {
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
    }
}