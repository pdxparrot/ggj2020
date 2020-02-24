using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public sealed class StartNodeData : ScriptNodeData
    {
        public override string Name => "Start";

        [Output("Next")]
        [SerializeField]
        [ReadOnly]
        private Guid _next;

        public Guid Next
        {
            get => _next;
            set => _next = value;
        }

        public StartNodeData() : base(new Rect(0.0f, 0.0f, 0.0f, 0.0f))
        {
        }
    }
}
