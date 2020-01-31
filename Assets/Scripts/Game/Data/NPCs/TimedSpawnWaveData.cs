using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    [CreateAssetMenu(fileName="TimedSpawnWaveData", menuName="pdxpartyparrot/Game/Data/Wave Spawner/TimedSpawnWave Data")]
    [Serializable]
    public class TimedSpawnWaveData : SpawnWaveData
    {
        [SerializeField]
        [Tooltip("The duration of the wave")]
        private float _duration;

        public float Duration => _duration;
    }
}
