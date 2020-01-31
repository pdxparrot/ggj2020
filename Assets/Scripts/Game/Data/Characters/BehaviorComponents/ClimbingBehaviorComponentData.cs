using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="ClimbingBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/ClimbingBehaviorComponent Data")]
    [Serializable]
    public class ClimbingBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        private LayerMask _raycastLayerMask;

        public LayerMask RaycastLayerMask => _raycastLayerMask;

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("How often to run raycast checks, in seconds")]
        private float _raycastRoutineRate = 0.1f;

        public float RaycastRoutineRate => _raycastRoutineRate;

        [Space(10)]

        [SerializeField]
        private float _attachDistance = 0.1f;

        public float AttachDistance => _attachDistance;

        [SerializeField]
        [Range(0, 10)]
        private float _armRayLength = 1.0f;

        public float ArmRayLength => _armRayLength;

        [SerializeField]
        [Range(0, 90)]
        private float _wrapAroundAngle = 45.0f;

        public float WrapAroundAngle => _wrapAroundAngle;

        [SerializeField]
        [Range(0, 10)]
        private float _headRayLength = 1.0f;

        public float HeadRayLength => _headRayLength;

        [SerializeField]
        [Range(0, 90)]
        private float _headRayAngle = 45.0f;

        public float HeadRayAngle => _headRayAngle;

        [SerializeField]
        [Range(0, 10)]
        private float _hangRayLength = 1.0f;

        public float HangRayLength => _hangRayLength;

        [SerializeField]
        [Range(0, 10)]
        private float _chestRayLength = 1.0f;

        public float ChestRayLength => _chestRayLength;

        [SerializeField]
        [Range(0, 50)]
        private float _climbSpeed = 1.0f;

        public float ClimbSpeed => _climbSpeed;

        [SerializeField]
        [Range(0, 50)]
        private float _hangSpeed = 1.0f;

        public float HangSpeed => _hangSpeed;

        [Space(10)]

#region Animations
        [Header("Animations")]

        [SerializeField]
        private string _climbingParam = "Climbing";

        public string ClimbingParam => _climbingParam;

        [SerializeField]
        private string _hangingParam = "Hanging";

        public string HangingParam => _hangingParam;

        [SerializeField]
        private float _wrapTimeSeconds = 1.0f;

        public float WrapTimeSeconds => _wrapTimeSeconds;

        [SerializeField]
        private float _climbUpTimeSeconds = 1.0f;

        public float ClimbUpTimeSeconds => _climbUpTimeSeconds;

        [SerializeField]
        private float _climbDownTimeSeconds = 1.0f;

        public float ClimbDownTimeSeconds => _climbDownTimeSeconds;

        [SerializeField]
        private float _hangTimeSeconds = 1.0f;

        public float HangTimeSeconds => _hangTimeSeconds;
#endregion
    }
}