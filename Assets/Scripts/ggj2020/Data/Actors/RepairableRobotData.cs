using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ggj2020.Data.Actors
{
    [CreateAssetMenu(fileName="RepairableRobotData", menuName="pdxpartyparrot/ggj2020/Data/RepairableRobot Data")]
    [Serializable]
    public sealed class RepairableRobotData : ScriptableObject
    {
// TODO: a lot of this probably should go to game data
        [SerializeField]
        [FormerlySerializedAs("_damagedAreasPerPlayerCount")]
        private int[] _initialDamagedAreasPerPlayerCount;

        public IReadOnlyCollection<int> InitialDamagedAreasPerPlayerCount => _initialDamagedAreasPerPlayerCount;

        [SerializeField]
        private int[] _damageAreaIncreasePerPlayerCount;

        public IReadOnlyCollection<int> DamageAreaIncreasePerPlayerCount => _damageAreaIncreasePerPlayerCount;

        [SerializeField]
        [Range(1, 100)]
        private int _damageAreaIncreaseBasePercent = 5;

        public int DamageAreaIncreaseBasePercent => _damageAreaIncreaseBasePercent;

        [SerializeField]
        [Range(1, 100)]
        private int _damageAreaIncreasePercentRate = 5;

        public int DamageAreaIncreaseRate => _damageAreaIncreasePercentRate;

        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float _impulseRate = 0.5f;

        public float ImpulseRate => _impulseRate;
    }
}
