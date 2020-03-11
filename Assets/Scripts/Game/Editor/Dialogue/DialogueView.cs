using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Editor.NodeEditor;
using pdxpartyparrot.Game.Data;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Game.Editor.Dialogue
{
    public sealed class DialogueView : NodeEditorView
    {
        public DialogueData DialogueData { get; private set; }

        public DialogueView(DialogueEditorWindow window) : base(window)
        {
        }

        public void LoadDialogue(DialogueData dialogueData)
        {
            DialogueData = dialogueData;

            DeleteElements(graphElements.ToList());

            //Debug.Log($"Loading dialogue '{dialogueData.name}' with {dialogueData.Nodes.Count} nodes...");

            // TODO
        }

        protected override NodeEditorNode CreateNode(NodeData nodeData)
        {
            //return new DialogueViewNode((DialogueNodeData)nodeData, Window.EdgeConnectorListener);
            return null;
        }

#region Event Handlers
        protected override void NodeCreationRequestEventHandler(NodeCreationContext context)
        {
            //CreateNodeWindow.ShowForCreate(this, context.screenMousePosition);
        }
#endregion
    }
}
