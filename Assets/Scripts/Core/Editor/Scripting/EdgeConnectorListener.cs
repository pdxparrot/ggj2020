using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class EdgeConnectorListener : IEdgeConnectorListener
    {
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.LogWarning("TODO: OnDropOutsidePort");
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            Debug.LogWarning("TODO: OnDrop");
        }
    }
}
