using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    [CreateAssetMenu(fileName="WaveSpawnData", menuName="pdxpartyparrot/Game/Data/Wave Spawner/Wave Spawn Data")]
    [Serializable]
    public class WaveSpawnData : ScriptableObject
    {
        [SerializeField]
        [ReorderableList]
        private SpawnWaveData.ReorderableList _waves = new SpawnWaveData.ReorderableList();

        public IReadOnlyCollection<SpawnWaveData> Waves => _waves.Items;
    }
}
