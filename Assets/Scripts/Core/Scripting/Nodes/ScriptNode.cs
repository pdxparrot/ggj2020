using pdxpartyparrot.Core.Data.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    public abstract class ScriptNode
    {
        public abstract void Init(ScriptRunner runner, IScriptNodeData data);

        public abstract void Run(ScriptContext context);
    }
}
