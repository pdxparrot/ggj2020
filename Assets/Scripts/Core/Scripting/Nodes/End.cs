using pdxpartyparrot.Core.Data.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    public sealed class End : ScriptNode
    {
        public override void Init(ScriptRunner runner, ScriptNodeData data)
        {
        }

        public override void Run(ScriptContext context)
        {
            context.Complete();
        }
    }
}
