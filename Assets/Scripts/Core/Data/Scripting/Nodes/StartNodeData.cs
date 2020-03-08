using System;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [ScriptNode("Start", typeof(StartNode), ScriptNodeAttribute.AllowedInstances.Single)]
    [Serializable]
    public sealed class StartNodeData : ScriptNodeData
    {
        [Connection("Next", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _next = ScriptNodePortData.Create();

        public ScriptNodePortData Next
        {
            get => _next;
            set => _next = value;
        }

        public StartNodeData() : base()
        {
        }

        public StartNodeData(Rect position) : base(position)
        {
        }
    }
}
