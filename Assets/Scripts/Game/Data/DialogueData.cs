using System;
using System.Collections.Generic;

using pdxpartyparrot.Game.Cinematics;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [CreateAssetMenu(fileName="DialogueData", menuName="pdxpartyparrot/Game/Data/Dialogue Data")]
    [Serializable]
    public sealed class DialogueData : ScriptableObject
    {
        [SerializeField]
        private Dialogue[] _dialoguePrefabs;

        public IReadOnlyCollection<Dialogue> DialoguePrefabs => _dialoguePrefabs;
    }
}
