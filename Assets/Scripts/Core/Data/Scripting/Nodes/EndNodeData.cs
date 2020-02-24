using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public sealed class EndNodeData : ScriptNodeData
    {
        public override string Name => "End";

        [Input]
        [SerializeField]
        [ReadOnly]
        private Guid _prev;

        public EndNodeData() : base(new Rect(500.0f, 0.0f, 0.0f, 0.0f))
        {
        }
    }
}
