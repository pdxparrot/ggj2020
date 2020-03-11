using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [Serializable]
    public struct NodeId
    {
        public static NodeId Create()
        {
            NodeId id = new NodeId();
            id.GenerateId();
            return id;
        }

        public static implicit operator int(NodeId id)
        {
            return id._id;
        }

        [SerializeField]
        [ReadOnly]
        private int _id;

        public bool IsValid => 0 != _id;

        public void GenerateId()
        {
            _id = new System.Random().Next(0, int.MaxValue);
        }
    }
}
