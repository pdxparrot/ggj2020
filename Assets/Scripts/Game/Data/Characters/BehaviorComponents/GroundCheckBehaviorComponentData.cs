using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="GroundCheckBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/GroundCheckBehaviorComponent Data")]
    [Serializable]
    public class GroundCheckBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        private LayerMask _raycastLayerMask;

        public LayerMask RaycastLayerMask => _raycastLayerMask;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("How often to run raycast checks, in seconds")]
        private float _raycastRoutineRate = 0.1f;

        public float RaycastRoutineRate => _raycastRoutineRate;

        [SerializeField]
        [Tooltip("Max distance from the ground that the character is considered grounded")]
        private float _GroundedEpsilon = 0.1f;

        public float GroundedEpsilon => _GroundedEpsilon;

        [SerializeField]
        [Tooltip("The length of the ground check sphere cast (useful for checking actual slope below the character)")]
        private float _groundCheckLength = 1.0f;

        public float GroundCheckLength => _groundCheckLength;

        [SerializeField]
        private float _slopeLimit = 30.0f;

        public float SlopeLimit => _slopeLimit;
    }
}