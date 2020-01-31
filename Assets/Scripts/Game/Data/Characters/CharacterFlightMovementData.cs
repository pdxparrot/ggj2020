using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters
{
    [CreateAssetMenu(fileName="CharacterFlightMovementData", menuName="pdxpartyparrot/Game/Data/CharacterFlightMovement Data")]
    [Serializable]
    public class CharacterFlightMovementData : ScriptableObject
    {
        [SerializeField]
        private LayerMask _collisionCheckLayerMask;

        public LayerMask CollisionCheckLayerMask => _collisionCheckLayerMask;

        [Space(10)]

#region Animations
        [Header("Animations")]

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

#region Movement
        [Header("Movement")]

        [SerializeField]
        private float _maxAttackAngle = 45.0f;

        public float MaxAttackAngle => _maxAttackAngle;

        [SerializeField]
        private float _maxBankAngle = 45.0f;

        public float MaxBankAngle => _maxBankAngle;

        [SerializeField]
        private float _rotationAnimationSpeed = 5.0f;

        public float RotationAnimationSpeed => _rotationAnimationSpeed;

        [SerializeField]
        private float _linearThrust = 10.0f;

        public float LinearThrust => _linearThrust;

        [SerializeField]
        private float _turnSpeed = 10.0f;

        public float TurnSpeed => _turnSpeed;

        [SerializeField]
        private float _terminalVelocity = 10.0f;

        public float TerminalVelocity => _terminalVelocity;
#endregion
    }
}
