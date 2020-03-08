using System;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptViewPort : Port
    {
        private ScriptViewNode _node;

        public ScriptViewNode Node => _node;

        private ScriptNodePortData _data;

        public Guid Id => _data.Id;

        public ScriptViewPort(ScriptViewNode node, ScriptNodePortData data, Orientation orientation, Direction direction, Capacity capacity, Type type) : base(orientation, direction, capacity, type)
        {
            _node = node;
            _data = data;
        }
    }
}
