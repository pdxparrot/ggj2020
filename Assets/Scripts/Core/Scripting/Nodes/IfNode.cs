using System;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    internal sealed class IfNode : ScriptNode
    {
        private IfNodeData NodeData => (IfNodeData)Data;

        [SerializeField]
        private ScriptNode _expression;

        [SerializeField]
        private ScriptNode _true;

        [SerializeField]
        private ScriptNode _false;

        public IfNode(ScriptNodeData nodeData) : base(nodeData)
        {
        }

        public override void Initialize(ScriptRunner runner)
        {
            _expression = runner.GetNode(NodeData.Expression.NodeId);
            _true = runner.GetNode(NodeData.True.NodeId);
            _false = runner.GetNode(NodeData.False.NodeId);
        }

        public override void Run(ScriptContext context)
        {
            bool result = _expression.GetOutputValue<bool>(NodeData.Expression.PortId);
            if(result) {
                context.Advance(_true);
            } else {
                context.Advance(_false);
            }
        }
    }
}
