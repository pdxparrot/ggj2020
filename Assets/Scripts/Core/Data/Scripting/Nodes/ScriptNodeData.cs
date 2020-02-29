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
        private int _id;

        public int Id => _id;

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
            GenerateId();
        }

        public ScriptNodeData(Rect position) : this()
        {
            Position = position;
        }

        public void GenerateId()
        {
            _id = new System.Random().Next(0, int.MaxValue);
        }
    }
}
