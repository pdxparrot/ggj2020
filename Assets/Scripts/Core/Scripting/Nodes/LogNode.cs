using System;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    internal sealed class LogNode : ScriptNode
    {
        private LogNodeData NodeData => (LogNodeData)Data;

        [SerializeField]
        private ScriptNode _next;

        public LogNode(ScriptNodeData nodeData) : base(nodeData)
        {
        }

        public override void Initialize(ScriptRunner runner)
        {
            _next = runner.GetNode(NodeData.Next.NodeId);
        }

        public override void Run(ScriptContext context)
        {
            Debug.LogFormat(NodeData.Level, LogOption.None, null, NodeData.Message);

            context.Advance(_next);
        }
    }
}
