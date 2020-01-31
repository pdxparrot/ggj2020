using System;

using pdxpartyparrot.Core.Data.Actors.Components;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters
{
    [Serializable]
    public abstract class CharacterBehaviorData : ActorBehaviorComponentData
    {
#region Physics
        [Header("Character Physics")]

        [SerializeField]
        [Range(0, 50)]
        [Tooltip("Move speed in m/s")]
        private float _moveSpeed = 30.0f;

        public float MoveSpeed => _moveSpeed;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("The animator-based threshold that we consider the character to be running")]
        private float _runThreshold = 0.75f;

        public float RunThreshold => _runThreshold;

        public float RunThresholdSquared => RunThreshold * RunThreshold;

        [SerializeField]
        [Range(0, 500)]
        [Tooltip("Add this many m/s to the player's fall speed, to make movement feel better without changing actual gravity")]
        private float _fallSpeedAdjustment = 200.0f;

        public float FallSpeedAdjustment => _fallSpeedAdjustment;

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("The characters terminal velocity in m/s")]
        private float _terminalVelocity = 50.0f;

        public float TerminalVelocity => _terminalVelocity;

        [SerializeField]
        [Tooltip("Allow movement while not grounded")]
        private bool _allowAirControl = true;

        public bool AllowAirControl => _allowAirControl;
#endregion

        [Space(10)]

#region Animations
        [Header("Character Animations")]

        [SerializeField]
        private string _moveXAxisParam = "InputX";

        public string MoveXAxisParam => _moveXAxisParam;

        [SerializeField]
        private string _moveZAxisParam = "InputZ";

        public string MoveZAxisParam => _moveZAxisParam;

        [SerializeField]
        private string _fallingParam = "Falling";

        public string FallingParam => _fallingParam;
#endregion

#region Action Buffering
        [Header("Action Buffering")]

        [SerializeField]
        private int _actionBufferSize = 1;

        public int ActionBufferSize => _actionBufferSize;

        [SerializeField]
        [Tooltip("Start clearing the action buffer after this many milliseconds without input")]
        private int _actionBufferTimeoutMs = 500;

        public int ActionBufferTimeoutMs => _actionBufferTimeoutMs;
#endregion
    }
}
