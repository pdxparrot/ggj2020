using System;

using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [ScriptNode("If", typeof(IfNode))]
    [Serializable]
    public sealed class IfNodeData : ScriptNodeData
    {
        [Connection("Prev", ConnectionAttribute.Direction.Input)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _prev = ScriptNodePortData.Create();

        public ScriptNodePortData Prev
        {
            get => _prev;
            set => _prev = value;
        }

        [Input("Expression", typeof(bool))]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _expression = ScriptNodePortData.Create();

        public ScriptNodePortData Expression => _expression;

        [Connection("True", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _true = ScriptNodePortData.Create();

        public ScriptNodePortData True
        {
            get => _true;
            set => _true = value;
        }

        [Connection("False", ConnectionAttribute.Direction.Output)]
        [SerializeField]
        [ReadOnly]
        private ScriptNodePortData _false = ScriptNodePortData.Create();

        public ScriptNodePortData False
        {
            get => _false;
            set => _false = value;
        }

        public IfNodeData(Rect position) : base(position)
        {
        }
    }
}
