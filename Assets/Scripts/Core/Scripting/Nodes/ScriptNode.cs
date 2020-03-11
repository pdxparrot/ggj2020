using System;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    [Serializable]
    public abstract class ScriptNode
    {
        [SerializeField]
        [ReadOnly]
        private ScriptNodeData _data;

        public ScriptNodeData Data => _data;

        public NodeId Id => Data.Id;

        public ScriptNode(ScriptNodeData nodeData)
        {
            _data = nodeData;
        }

        public abstract void Initialize(ScriptRunner runner);

        public abstract void Run(ScriptContext context);

        public virtual T GetOutputValue<T>(Guid portid)
        {
            return default;
        }
    }
}
