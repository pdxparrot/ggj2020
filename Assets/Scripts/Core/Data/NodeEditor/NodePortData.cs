using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [Serializable]
    public abstract class NodePortData
    {
        [SerializeField]
        [ReadOnly]
        private Guid _id;

        public Guid Id => _id;

        [SerializeField]
        [ReadOnly]
        private NodeId _nodeId;

        public NodeId NodeId
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

        public void GenerateId()
        {
            _id = Guid.NewGuid();
        }
    }
}
