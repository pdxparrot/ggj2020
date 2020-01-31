using pdxpartyparrot.Core.Data.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting.Nodes
{
    public sealed class Start : ScriptNode
    {
        [Output]
        private ScriptNode _next;

        public override void Init(ScriptRunner runner, ScriptNodeData data)
        {
            StartData startData = data as StartData;
            if(null == startData) {
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
