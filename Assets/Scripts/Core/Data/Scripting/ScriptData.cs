using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Data.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting
{
    //[CreateAssetMenu(fileName="ScriptData", menuName="pdxpartyparrot/Core/Data/Script Data")]
    [Serializable]
    public sealed class ScriptData : ScriptableObject
    {
        [SerializeField]
        private /*readonly*/ ScriptNodeData[] _nodes;

        public IReadOnlyCollection<ScriptNodeData> Nodes => _nodes;
    }
}
