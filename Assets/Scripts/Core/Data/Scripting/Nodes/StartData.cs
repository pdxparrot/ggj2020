using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public sealed class StartData : ScriptNodeData
    {
        [SerializeField]
        private Guid _next;

        public Guid Next
        {
            get => _next;
            set => _next = value;
        }
    }
}
