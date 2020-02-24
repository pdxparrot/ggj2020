using System;

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
        private Guid _id;

        public Guid Id => _id;

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
            _id = Guid.NewGuid();
        }

        public ScriptNodeData(Rect position) : this()
        {
            Position = position;
        }
    }
}
