using System;

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

        [Connection("Next", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _next = ScriptNodePortData.Create();

        public ScriptNodePortData Next => _next;

        [SerializeField]
        [ReadOnly]
        private LogType _level;

        public LogType Level => _level;

        [SerializeField]
        [ReadOnly]
        private string _message;

        public string Message => _message;

        public LogNodeData(Rect position) : base(position)
        {
        }
    }
}
