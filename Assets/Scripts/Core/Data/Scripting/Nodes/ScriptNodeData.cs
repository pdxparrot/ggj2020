using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [Serializable]
    public abstract class ScriptNodeData
    {
        [SerializeField]
        private Guid _id;

        public Guid Id => _id;

        [SerializeField]
        private Rect _rect;

        public Rect Rect => _rect;

        protected ScriptNodeData()
        {
            _id = Guid.NewGuid();
        }

        protected ScriptNodeData(Guid id)
        {
            _id = id;
        }
    }
}
