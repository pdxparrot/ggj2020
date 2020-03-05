using pdxpartyparrot.Core.Data.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    public sealed class NoOpNode : ScriptNode
    {
        private ScriptNode _next;

        public override void Init(ScriptRunner runner, ScriptNodeData data)
        {
            if(!(data is NoOpNodeData nodeData)) {
                return;
            }

            _next = runner.GetNode(nodeData.Next);
        }

        public override void Run(ScriptContext context)
        {
            context.Advance();
        }
    }
}
