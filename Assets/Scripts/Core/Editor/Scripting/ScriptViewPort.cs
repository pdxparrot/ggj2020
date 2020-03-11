using System;

using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.NodeEditor;

using UnityEditor.Experimental.GraphView;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptViewPort : NodeEditorPort
    {
        public ScriptViewPort(ScriptViewNode node, ScriptNodePortData portData, Orientation orientation, Direction direction, Capacity capacity, Type type) : base(node, portData, orientation, direction, capacity, type)
        {
        }
    }
}
