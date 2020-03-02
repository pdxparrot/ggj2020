using System;
using System.Reflection;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    public interface IScriptNodeData
    {
        string Name { get; }

        ScriptNodeId Id { get; }

        Rect Position { get; }
    }

    [Serializable]
    public abstract class ScriptNodeData<T> : IScriptNodeData where T: ScriptNodeData<T>
    {
        public static string NodeName => ((ScriptNodeAttribute)typeof(T).GetCustomAttribute(typeof(ScriptNodeAttribute))).Name;

        public string Name => NodeName;

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
