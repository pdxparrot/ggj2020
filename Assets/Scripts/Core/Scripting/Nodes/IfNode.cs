using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    internal sealed class IfNode : ScriptNode
    {
        private IfNodeData NodeData => (IfNodeData)Data;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _expression;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _true;

        [SerializeField]
        [CanBeNull]
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
            bool result = _expression?.GetOutputValue<bool>(NodeData.Expression.PortId) ?? false;
            if(result) {
                context.Advance(_true);
            } else {
                context.Advance(_false);
            }
        }
    }
}
