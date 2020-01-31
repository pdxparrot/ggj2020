using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="SpawnData", menuName="pdxpartyparrot/Core/Data/Spawn Data")]
    [Serializable]
    public class SpawnData : ScriptableObject
    {
        public enum SpawnMethod
        {
            Random,
            RoundRobin
        }

        [Serializable]
        public struct SpawnPointType
        {
            [SerializeField]
            private string _tag;

            public string Tag => _tag;

            [SerializeField]
            private SpawnMethod _spawnMethod;

            public SpawnMethod SpawnMethod => _spawnMethod;
        }

        [SerializeField]
        private string[] _playerSpawnPointTags;

        public IReadOnlyCollection<string> PlayerSpawnPointTags => _playerSpawnPointTags;

        [SerializeField]
        private SpawnPointType[] _types;

        public IReadOnlyCollection<SpawnPointType> Types => _types;
    }
}
