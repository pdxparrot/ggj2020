using System;
using System.Reflection;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public abstract class ScriptNodeData
    {
        [SerializeField]
        [ReadOnly]
        private string _name;

        public string Name => _name;

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
            set => _position = value;
        }

        protected ScriptNodeData()
        {
            _name = GetType().GetCustomAttribute<ScriptNodeAttribute>()?.Name ?? "Invalid Script Node";

            _id = ScriptNodeId.Create();
        }

        public ScriptNodeData(Rect position) : this()
        {
            Position = position;
        }
    }
}
