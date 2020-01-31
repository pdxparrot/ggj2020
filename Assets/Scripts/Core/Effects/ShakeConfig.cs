using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects
{
    [Serializable]
    public class ShakeConfig
    {
        [SerializeField]
        private float _duration = 1.0f;

        public float Duration => _duration;

        [SerializeField]
        private Vector3 _strength = new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3 Strength => _strength;

        [SerializeField]
        private int _vibrato = 1;

        public int Vibrato => _vibrato;

        [SerializeField]
        [Range(0.0f, 180.0f)]
        [Tooltip("Values over 90 kind of suck")]
        private float _randomness = 45.0f;

        public float Randomness => _randomness;

        [SerializeField]
        private bool _fadeOut = true;

        public bool FadeOut => _fadeOut;
    }
}
