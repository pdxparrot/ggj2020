using System;
using System.Collections.Generic;

using pdxpartyparrot.Game.Cinematics;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [CreateAssetMenu(fileName="CinematicsData", menuName="pdxpartyparrot/Game/Data/Cinematics Data")]
    [Serializable]
    public sealed class CinematicsData : ScriptableObject
    {
        [SerializeField]
        private Cinematic[] _cinematicsPrefabs;

        public IReadOnlyCollection<Cinematic> CinematicsPrefabs => _cinematicsPrefabs;
    }
}
