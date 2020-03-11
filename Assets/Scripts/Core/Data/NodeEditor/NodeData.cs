using System;
using System.Reflection;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [Serializable]
    public abstract class NodeData
    {
        [SerializeField]
        [ReadOnly]
        private string _title;

        public string Title => _title;

        [SerializeField]
        [ReadOnly]
        private NodeId _id = NodeId.Create();

        public NodeId Id => _id;

        [SerializeField]
        [ReadOnly]
        private Rect _position;

        public Rect Position
        {
            get => _position;
            set => _position = value;
        }

        protected NodeData()
        {
            _title = GetType().GetCustomAttribute<NodeAttribute>()?.Name ?? "Invalid Script Node";
        }

        public NodeData(Rect position) : this()
        {
            Position = position;
        }
    }
}
