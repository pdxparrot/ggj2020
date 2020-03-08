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
            // TODO: is there a way to make this work when we don't know which port to connect to?
            /*ScriptViewPort port = (ScriptViewPort)(edge.output ?? edge.input);
            bool isOutput = null != edge.output;

            CreateNodeWindow.ShowForDrop(_scriptView, position, node => {
                if(isOutput) {
                    _scriptView.CreateEdge(port.Node.Id, port.Id, node.Id, ???);
                } else {
                    _scriptView.CreateEdge(node.Id, ???, port.Node.Id, port.Id);
                }
            });*/
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            ScriptViewPort output = (ScriptViewPort)edge.output;
            ScriptViewPort input = (ScriptViewPort)edge.input;
             _scriptView.CreateEdge(output.Node.Id, output.Id, input.Node.Id, input.Id);
        }
    }
}
