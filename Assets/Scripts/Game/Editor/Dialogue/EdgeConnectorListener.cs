using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace pdxpartyparrot.Game.Editor.Dialogue
{
    public sealed class EdgeConnectorListener : IEdgeConnectorListener
    {
        private DialogueView _dialogueView;

        public EdgeConnectorListener(DialogueView dialogueView)
        {
            _dialogueView = dialogueView;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            // TODO
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            // TODO
        }
    }
}
