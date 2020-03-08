using System;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public struct ScriptNodePortData
    {
        [SerializeField]
        [ReadOnly]
        private Guid _id;

        public Guid Id => _id;

        [SerializeField]
        [ReadOnly]
        private ScriptNodeId _nodeId;

        public ScriptNodeId NodeId
        {
            get => _nodeId;
            set => _nodeId = value;
        }

        [SerializeField]
        [ReadOnly]
        private Guid _portId;

        public Guid PortId
        {
            get => _portId;
            set => _portId = value;
        }

        public bool IsConnected => NodeId.IsValid && Guid.Empty != PortId;
    }
}
