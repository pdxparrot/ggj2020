using System;

using pdxpartyparrot.Core.Data.NodeEditor;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public abstract class ScriptNodeData : NodeData
    {
        protected ScriptNodeData() : base()
        {
        }

        public ScriptNodeData(Rect position) : base(position)
        {
        }
    }
}
