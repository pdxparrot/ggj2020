using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Input
{
    [Serializable]
    public class RumbleConfig
    {
        [SerializeField]
        [Range(0.0f, 5.0f)]
        private float _seconds = 0.5f;

        public float Seconds => _seconds;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _lowFrequency = 0.5f;

        public float LowFrequency => _lowFrequency;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _highFrequency = 0.5f;

        public float HighFrequency => _highFrequency;
    }
}
