using System;

using pdxpartyparrot.Core.Data.NodeEditor;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public sealed class ScriptNodePortData : NodePortData
    {
        public static ScriptNodePortData Create()
        {
            ScriptNodePortData data = new ScriptNodePortData();
            data.GenerateId();
            return data;
        }
    }
}
