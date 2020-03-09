using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    internal sealed class LogNode : ScriptNode
    {
        private LogNodeData NodeData => (LogNodeData)Data;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _level;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _message;

        [SerializeField]
        [CanBeNull]
        private ScriptNode _next;

        public LogNode(ScriptNodeData nodeData) : base(nodeData)
        {
        }

        public override void Initialize(ScriptRunner runner)
        {
            _level = runner.GetNode(NodeData.Level.NodeId);
            _message = runner.GetNode(NodeData.Message.NodeId);
            _next = runner.GetNode(NodeData.Next.NodeId);
        }

        public override void Run(ScriptContext context)
        {
            LogType level = _level?.GetOutputValue<LogType>(NodeData.Level.PortId) ?? LogType.Log;
            string message = _message?.GetOutputValue<string>(NodeData.Message.PortId) ?? string.Empty;
            Debug.LogFormat(level, LogOption.None, null, message);

            context.Advance(_next);
        }
    }
}
