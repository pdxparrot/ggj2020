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
        private ScriptNodeId _prev;

        public ScriptNodeId Prev
        {
            get => _prev;
            set => _prev = value;
        }

        [Connection("Next", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodeId _next;

        public ScriptNodeId Next
        {
            get => _next;
            set => _next = value;
        }

        public NoOpNodeData(Rect position) : base(position)
        {
        }
    }
}
