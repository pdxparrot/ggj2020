using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting
{
    [CreateAssetMenu(fileName="ScriptData", menuName="pdxpartyparrot/Core/Data/Script Data")]
    [Serializable]
    public sealed class ScriptData : ScriptableObject
    {
        [SerializeReference]
        [ReadOnly]
        private /*readonly*/ ScriptNodeData[] _nodes = { new StartNodeData() };

        public IReadOnlyCollection<ScriptNodeData> Nodes => _nodes;
    }
}
