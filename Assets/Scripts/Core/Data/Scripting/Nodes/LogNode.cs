using System;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [ScriptNode("Log", typeof(LogNode))]
    [Serializable]
    public sealed class LogNodeData : ScriptNodeData
    {
        [Connection("Prev", ConnectionAttribute.Direction.Input)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _prev = ScriptNodePortData.Create();

        public ScriptNodePortData Prev => _prev;

        [Input("Level", typeof(LogType))]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _level = ScriptNodePortData.Create();

        public ScriptNodePortData Level => _level;

        [Input("Message", typeof(string))]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _message = ScriptNodePortData.Create();

        public ScriptNodePortData Message => _message;

        [Connection("Next", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _next = ScriptNodePortData.Create();

        public ScriptNodePortData Next => _next;

        public LogNodeData(Rect position) : base(position)
        {
        }
    }
}
