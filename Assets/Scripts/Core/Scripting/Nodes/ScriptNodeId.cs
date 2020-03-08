using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    public struct ScriptNodeId
    {
        public static ScriptNodeId Create()
        {
            ScriptNodeId id = new ScriptNodeId();
            id.GenerateId();
            return id;
        }

        public static implicit operator int(ScriptNodeId id)
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
