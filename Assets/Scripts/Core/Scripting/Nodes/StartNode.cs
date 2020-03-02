using pdxpartyparrot.Core.Data.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    public sealed class StartNode : ScriptNode
    {
        private ScriptNode _next;

        public override void Init(ScriptRunner runner, IScriptNodeData data)
        {
            if(!(data is StartNodeData startData)) {
                return;
            }

            _next = runner.GetNode(startData.Next);
        }

        public override void Run(ScriptContext context)
        {
            context.Advance();
        }
    }
}
