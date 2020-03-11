using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [CreateAssetMenu(fileName="DialogueData", menuName="pdxpartyparrot/Game/Data/Dialogue Data")]
    [Serializable]
    public sealed class DialogueData : ScriptableObject
    {
#if false
        [SerializeReference]
        [ReadOnly]
        private /*readonly*/ DialogueNodeData[] _nodes = { new StartNodeData() };

        public IReadOnlyCollection<DialogueNodeData> Nodes => _nodes;
#endif
    }
}
