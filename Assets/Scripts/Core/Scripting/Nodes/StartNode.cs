using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    internal sealed class StartNode : ScriptNode
    {
        private StartNodeData NodeData => (StartNodeData)Data;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _next;

        public StartNode(ScriptNodeData nodeData) : base(nodeData)
        {
        }

        public override void Initialize(ScriptRunner runner)
        {
            _next = runner.GetNode(NodeData.Next.NodeId);
        }

        public override void Run(ScriptContext context)
        {
            context.Advance(_next);
        }
    }
}
