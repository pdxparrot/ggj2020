using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class EdgeConnectorListener : IEdgeConnectorListener
    {
        private ScriptView _scriptView;

        public EdgeConnectorListener(ScriptView scriptView)
        {
            _scriptView = scriptView;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            CreateNodeWindow.ShowForDrop(_scriptView, position, nodeData => {
                Debug.LogWarning("TODO: Add edge to new node");
            });
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            Debug.LogWarning("TODO: OnDrop");
        }
    }
}
