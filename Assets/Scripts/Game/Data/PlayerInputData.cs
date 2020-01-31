using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data
{
    [Serializable]
    public abstract class PlayerInputData : ScriptableObject
    {
        [SerializeField]
        private float _movementLerpSpeed = 5.0f;

        public float MovementLerpSpeed => _movementLerpSpeed;

        [SerializeField]
        private float _lookLerpSpeed = 10.0f;

        public float LookLerpSpeed => _lookLerpSpeed;

        [Space(10)]

#region Input Buffering
        [Header("Input Buffering")]

        [SerializeField]
        private int _inputBufferSize = 1;

        public int InputBufferSize => _inputBufferSize;

        [SerializeField]
        [Tooltip("Start clearing the input buffer after this many milliseconds without input")]
        private int _inputBufferTimeoutMs = 500;

        public int InputBufferTimeoutMs => _inputBufferTimeoutMs;
#endregion
    }
}
