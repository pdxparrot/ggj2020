using System;
using System.Linq;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [ScriptNode("Start", ScriptNodeAttribute.AllowedInstances.Single)]
    [Serializable]
    public sealed class StartNodeData : ScriptNodeData<StartNodeData>
    {
        [Connection("Next")]
        [SerializeField]
        [ReadOnly]
        private ScriptNodeId _next;

        public ScriptNodeId Next
        {
            get => _next;
            set => _next = value;
        }

        public StartNodeData() : base(new Rect(0.0f, 0.0f, 0.0f, 0.0f))
        {
        }

        public StartNodeData(Rect position) : base(position)
        {
        }
    }
}
