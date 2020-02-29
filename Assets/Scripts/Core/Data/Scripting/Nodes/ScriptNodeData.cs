using System;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public abstract class ScriptNodeData
    {
        public abstract string Name { get; }

        [SerializeField]
        [ReadOnly]
        private ScriptNodeId _id;

        public ScriptNodeId Id => _id;

        [SerializeField]
        [ReadOnly]
        private Rect _position;

        public Rect Position
        {
            get => _position;
            protected set => _position = value;
        }

        protected ScriptNodeData()
        {
            _id = ScriptNodeId.Create();
        }

        public ScriptNodeData(Rect position) : this()
        {
            Position = position;
        }
    }
}
