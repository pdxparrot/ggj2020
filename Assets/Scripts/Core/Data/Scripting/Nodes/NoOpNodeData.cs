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
        private ScriptNodePortData _prev;

        public ScriptNodePortData Prev
        {
            get => _prev;
            set => _prev = value;
        }

        [Connection("Next", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _next;

        public ScriptNodePortData Next
        {
            get => _next;
            set => _next = value;
        }

        public NoOpNodeData(Rect position) : base(position)
        {
        }
    }
}
