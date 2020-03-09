using System;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [ScriptNode("NoOp", typeof(NoOpNode))]
    [Serializable]
    public sealed class NoOpNodeData : ScriptNodeData
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

        public NoOpNodeData(Rect position) : base(position)
        {
        }
    }
}
