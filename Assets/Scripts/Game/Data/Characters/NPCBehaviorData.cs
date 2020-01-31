using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters
{
    [Serializable]
    public abstract class NPCBehaviorData : CharacterBehaviorData
    {
#region Physics
        [Header("NPC Physics")]

        [SerializeField]
        [Range(0, 360)]
        [Tooltip("Angular move speed in deg/s")]
        private float _angularMoveSpeed = 120.0f;

        public float AngularMoveSpeed => _angularMoveSpeed;

        [SerializeField]
        [Range(0, 50)]
        [Tooltip("Move acceleration in m/s^2")]
        private float _moveAcceleration = 10.0f;

        public float MoveAcceleration => _moveAcceleration;

        [SerializeField]
        [Range(0, 50)]
        [Tooltip("Stopping distance in meters")]
        private float _stoppingDistance = 0.1f;

        public float StoppingDistance => _stoppingDistance;
#endregion
    }
}
