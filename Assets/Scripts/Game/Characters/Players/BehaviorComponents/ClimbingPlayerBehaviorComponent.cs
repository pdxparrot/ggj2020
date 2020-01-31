using System.Collections;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;
using pdxpartyparrot.Game.World;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    public sealed class ClimbingPlayerBehaviorComponent : PlayerBehaviorComponent
    {
#region Actions
        public class GrabAction : CharacterBehaviorAction
        {
            public static GrabAction Default = new GrabAction();
        }

        public class ReleaseAction : CharacterBehaviorAction
        {
            public static ReleaseAction Default = new ReleaseAction();
        }
#endregion

        [SerializeField]
        private ClimbingBehaviorComponentData _data;

        private enum ClimbMode
        {
            None,
            Climbing,
            Hanging
        }

        [SerializeField]
        [ReadOnly]
        private ClimbMode _climbMode = ClimbMode.None;

        public bool IsClimbing => _climbMode != ClimbMode.None;

// TODO: the number of constant raycasts could probably be reduced even more
// TODO: we probably also don't need to hold onto the hit results, just update and handle them one at a time

        [Space(10)]

#region Hands
        [Header("Hands")]

        [SerializeField]
        private Transform _leftHandTransform;

        private RaycastHit? _leftHandHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didLeftHandRaycast;

        [SerializeField]
        [ReadOnly]
        private bool _didWrapLeftRaycast;

        [SerializeField]
        [ReadOnly]
        private bool _didRotateLeftRaycast;

        private RaycastHit? _leftHandHangHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didLeftHandHangRaycast;

        private bool CanGrabLeft => null != _leftHandHitResult;

        private bool CanHangLeft => null != _leftHandHangHitResult;

        [SerializeField]
        private Transform _rightHandTransform;

        private RaycastHit? _rightHandHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didRightHandRaycast;

        [SerializeField]
        [ReadOnly]
        private bool _didWrapRightRaycast;

        [SerializeField]
        [ReadOnly]
        private bool _didRotateRightRaycast;

        private RaycastHit? _rightHandHangHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didRightHandHangRaycast;

        private bool CanGrabRight => null != _rightHandHitResult;

        private bool CanHangRight => null != _rightHandHangHitResult;
#endregion

        [SerializeField]
        private Transform _hangTransform;

        [Space(10)]

#region Head
        [Header("Head")]

        [SerializeField]
        private Transform _headTransform;

        private RaycastHit? _headHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didHeadRaycast;

        [SerializeField]
        [ReadOnly]
        private bool _didClimbUpRaycast;
#endregion

        [Space(10)]

#region Chest
        [Header("Chest")]

        [SerializeField]
        private Transform _chestTransform;

        private RaycastHit? _chestHitResult;

        [SerializeField]
        [ReadOnly]
        private bool _didChestRaycast;
#endregion

        private bool CanClimbUp => IsClimbing && (null == _headHitResult && null != _chestHitResult);

        private GroundCheckBehaviorComponent _groundChecker;

        private Coroutine _raycastCoroutine;

        [Space(10)]

#region Debug
        [Header("Debug")]

        [SerializeField]
        [Tooltip("Debug break when grabbing fails")]
        private bool _breakOnFall;

        private DebugMenuNode _debugMenuNode;
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsNotNull(Behavior.Owner.ManualAnimator, "ClimbingBehaviorComponent requires a manual animator");
            Assert.IsTrue(Mathf.Approximately(_leftHandTransform.position.y, _rightHandTransform.position.y), "Character hands are at different heights!");
            Assert.IsTrue(_headTransform.position.y > _leftHandTransform.position.y, "Character head should be above player hands!");
            Assert.IsTrue(_chestTransform.position.y < _leftHandTransform.position.y, "Character chest should be below player hands!");

            _groundChecker = Behavior.GetBehaviorComponent<GroundCheckBehaviorComponent>();
            Assert.IsNotNull(_groundChecker, "ClimbingBehaviorComponent requires a ground checker");

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }

        private void OnEnable()
        {
            _raycastCoroutine = StartCoroutine(RaycastRoutine());
        }

        private void OnDisable()
        {
            if(null != _raycastCoroutine) {
                StopCoroutine(_raycastCoroutine);
                _raycastCoroutine = null;
            }
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

// TODO: encapsulate the math here so we a) don't duplicate it in the raycast methods and b) guarantee we always match the math done in the raycast methods

            // left hand
            if(_didLeftHandRaycast) {
                Gizmos.color = null != _leftHandHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_leftHandTransform.position, _leftHandTransform.position + transform.forward * _data.ArmRayLength);
            }

            if(_didLeftHandHangRaycast) {
                Gizmos.color = null != _leftHandHangHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_leftHandTransform.position, _leftHandTransform.position + (transform.up) * _data.HangRayLength);
            }

            if(_didWrapLeftRaycast) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_leftHandTransform.position, _leftHandTransform.position + (Quaternion.AngleAxis(_data.WrapAroundAngle, transform.up) * transform.forward) * _data.ArmRayLength * 2.0f);
            }

            if(_didRotateLeftRaycast) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_leftHandTransform.position, _leftHandTransform.position + (Quaternion.AngleAxis(-90.0f, transform.up) * transform.forward) * _data.ArmRayLength * 0.5f);
            }

            // right hand
            if(_didRightHandRaycast) {
                Gizmos.color = null != _rightHandHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_rightHandTransform.position, _rightHandTransform.position + transform.forward * _data.ArmRayLength);
            }

            if(_didRightHandHangRaycast) {
                Gizmos.color = null != _rightHandHangHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_rightHandTransform.position, _rightHandTransform.position + (transform.up) * _data.HangRayLength);
            }

            if(_didWrapRightRaycast) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_rightHandTransform.position, _rightHandTransform.position + (Quaternion.AngleAxis(-_data.WrapAroundAngle, transform.up) * transform.forward) * _data.ArmRayLength * 2.0f);
            }

            if(_didRotateRightRaycast) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(_rightHandTransform.position, _rightHandTransform.position + (Quaternion.AngleAxis(90.0f, transform.up) * transform.forward) * _data.ArmRayLength * 0.5f);
            }

            // head
            if(_didHeadRaycast) {
                Gizmos.color = null != _headHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_headTransform.position, _headTransform.position + (Quaternion.AngleAxis(-_data.HeadRayAngle, transform.right) * transform.forward) * _data.HeadRayLength);
            }

            if(_didClimbUpRaycast) {
                Gizmos.color = Color.white;
                Vector3 start = _headTransform.position + (Quaternion.AngleAxis(-_data.HeadRayAngle, transform.right) * transform.forward) * _data.HeadRayLength;
                Vector3 end = start + Behavior.Owner.Height * -Vector3.up;
                Gizmos.DrawLine(start, end);
            }

            // chest
            if(_didChestRaycast) {
                Gizmos.color = null != _chestHitResult ? Color.red : Color.yellow;
                Gizmos.DrawLine(_chestTransform.position, _chestTransform.position + transform.forward * _data.ChestRayLength);
            }
        }
#endregion

        public override bool OnAnimationUpdate(float dt)
        {
            if(!IsClimbing) {
                return false;
            }

            switch(_climbMode)
            {
            case ClimbMode.Climbing:
            case ClimbMode.Hanging:
                break;
            }

            if(null != Behavior.Animator) {
                Behavior.Animator.SetFloat(Behavior.CharacterBehaviorData.MoveXAxisParam, Behavior.CanMove ? Mathf.Abs(PlayerBehavior.MoveDirection.x) : 0.0f);
                Behavior.Animator.SetFloat(Behavior.CharacterBehaviorData.MoveZAxisParam, Behavior.CanMove ? Mathf.Abs(PlayerBehavior.MoveDirection.y) : 0.0f);
            }

            return true;
        }

        public override bool OnPhysicsUpdate(float dt)
        {
            if(!IsClimbing) {
                return false;
            }

            switch(_climbMode)
            {
            case ClimbMode.Climbing:
                Vector3 velocity = Behavior.Owner.Movement.Rotation * (PlayerBehavior.MoveDirection * _data.ClimbSpeed);
                if(_groundChecker.DidGroundCheckCollide && velocity.y < 0.0f) {
                    velocity.y = 0.0f;
                }
                Behavior.Owner.Movement.Move(velocity * dt);
                return true;
            case ClimbMode.Hanging:
                break;
            }

            return false;
        }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(action is GrabAction) {
                if(IsClimbing) {
                    return true;
                }

                if(CanGrabLeft && CanGrabRight) {
                    StartClimbing();

                    if(null != _leftHandHitResult) {
                        AttachToSurface(_leftHandHitResult.Value, false);
                    } else if(null != _rightHandHitResult) {
                        AttachToSurface(_rightHandHitResult.Value, false);
                    }
                } else if(CanHangLeft && CanHangRight) {
                    StartHanging();

                    if(null != _leftHandHangHitResult) {
                        AttachToSurface(_leftHandHangHitResult.Value, true);
                    } else if(null != _rightHandHangHitResult) {
                        AttachToSurface(_rightHandHangHitResult.Value, true);
                    }
                } else {
                    return false;
                }

                return true;
            }

            if(action is ReleaseAction) {
                if(IsClimbing) {
                    StopClimbing();
                }

                return true;
            }

            return false;
        }

        private void StartClimbing()
        {
            _climbMode = ClimbMode.Climbing;
            Behavior.Owner.Movement.UseGravity = false;

            if(null != Behavior.Animator) {
                Behavior.Animator.SetBool(_data.ClimbingParam, true);
                Behavior.Animator.SetBool(_data.HangingParam, false);
            }
        }

        private void StartHanging()
        {
            _climbMode = ClimbMode.Hanging;
            Behavior.Owner.Movement.UseGravity = false;

            if(null != Behavior.Animator) {
                Behavior.Animator.SetBool(_data.ClimbingParam, false);
                Behavior.Animator.SetBool(_data.HangingParam, true);
            }
        }

        public void StopClimbing()
        {
            _climbMode = ClimbMode.None;
            Behavior.Owner.Movement.UseGravity = true;

            if(null != Behavior.Animator) {
                Behavior.Animator.SetBool(_data.ClimbingParam, false);
                Behavior.Animator.SetBool(_data.HangingParam, false);
            }

            // fix our orientation, just in case
            Vector3 rotation = transform.eulerAngles;
            rotation.x = rotation.z = 0.0f;
            transform.eulerAngles = rotation;
        }

        private IEnumerator RaycastRoutine()
        {
            Debug.Log($"Starting climbing raycast routine for {Behavior.Owner.Id}");

            WaitForSeconds wait = new WaitForSeconds(_data.RaycastRoutineRate);
            while(true) {
                UpdateRaycasts();

                HandleRaycasts();

                yield return wait;
            }
        }

        private void UpdateRaycasts()
        {
            ResetRaycastDebug();

            if(Behavior.Owner.ManualAnimator.IsAnimating) {
                return;
            }

            Profiler.BeginSample("ClimbingBehaviorComponent.UpdateRaycasts");
            try {
                UpdateHandRaycasts();
                UpdateHeadRaycasts();
                UpdateChestRaycasts();
            } finally {
                Profiler.EndSample();
            }
        }

        private void ResetRaycastDebug()
        {
            // left hand
            _didLeftHandRaycast = false;
            _didWrapLeftRaycast = false;
            _didRotateLeftRaycast = false;
            _didLeftHandHangRaycast = false;

            // right hand
            _didRightHandRaycast = false;
            _didRotateRightRaycast = false;
            _didWrapRightRaycast = false;
            _didRightHandHangRaycast = false;

            // head
            _didHeadRaycast = false;
            _didClimbUpRaycast = false;

            // chest
            _didChestRaycast = false;
        }

#region Hand Raycasts
        private void UpdateHandRaycasts()
        {
            UpdateLeftHandRaycasts();
            UpdateRightHandRaycasts();
        }

        private void UpdateLeftHandRaycasts()
        {
            _didLeftHandRaycast = true;

            _leftHandHitResult = null;

            RaycastHit hit;
            if(Physics.Raycast(_leftHandTransform.position, transform.forward, out hit, _data.ArmRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _leftHandHitResult = hit;
                }
            }

            _didLeftHandHangRaycast = true;

            _leftHandHangHitResult = null;

            if(Physics.Raycast(_leftHandTransform.position, transform.up, out hit, _data.HangRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _leftHandHangHitResult = hit;
                }
            }
        }

        private void UpdateRightHandRaycasts()
        {
            _didRightHandRaycast = true;

            _rightHandHitResult = null;

            if(Physics.Raycast(_rightHandTransform.position, transform.forward, out var hit, _data.ArmRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _rightHandHitResult = hit;
                }
            }

            _didRightHandHangRaycast = true;

            _rightHandHangHitResult = null;

            if(Physics.Raycast(_rightHandTransform.position, transform.up, out hit, _data.HangRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _rightHandHangHitResult = hit;
                }
            }
        }
#endregion

#region Head Raycasts
        private void UpdateHeadRaycasts()
        {
            if(!IsClimbing) {
                return;
            }

            _didHeadRaycast = true;

            _headHitResult = null;

            if(Physics.Raycast(_headTransform.position, Quaternion.AngleAxis(-_data.HeadRayAngle, transform.right) * transform.forward, out var hit, _data.HeadRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _headHitResult = hit;
                }
            }
        }
#endregion

#region Chest Raycasts
        private void UpdateChestRaycasts()
        {
            if(!IsClimbing) {
                return;
            }

            _didChestRaycast = true;

            _chestHitResult = null;

            if(Physics.Raycast(_chestTransform.position, transform.forward, out var hit, _data.ChestRayLength, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
                if(null != grabbable) {
                    _chestHitResult = hit;
                }
            }
        }
#endregion

        private void HandleRaycasts()
        {
            Profiler.BeginSample("ClimbingBehaviorComponent.HandleRaycasts");
            try {
                if(IsClimbing) {
                    HandleClimbingRaycasts();
                }
            } finally {
                Profiler.EndSample();
            }
        }

        private void HandleClimbingRaycasts()
        {
            if(Behavior.Owner.ManualAnimator.IsAnimating) {
                return;
            }

            switch(_climbMode)
            {
            case ClimbMode.None:
                break;
            case ClimbMode.Climbing:
                if(CanGrabLeft && CanGrabRight) {
                    if(!CheckHang()) {
                        if(!CheckRotateLeft()) {
                            CheckRotateRight();
                        }
                    }
                } else if(!CanGrabLeft && CanGrabRight) {
                    CheckWrapLeft();
                } else if(CanGrabLeft && !CanGrabRight) {
                    CheckWrapRight();
                } else /*if(!CanGrabLeft && !CanGrabRight)*/ {
                    if(CanClimbUp) {
                        CheckClimbUp();
                    } else {
                        Debug.LogWarning("Unexpectedly fell off!");
                        StopClimbing();

                        if(_breakOnFall) {
                            Debug.Break();
                        }
                    }
                }
                break;
            case ClimbMode.Hanging:
                if(CanHangLeft && CanHangRight) {
                    CheckClimb();
                } else if(!CanHangLeft && CanHangRight) {
                    // TODO: climb up?
                } else if(CanHangLeft && !CanHangRight) {
                    // TODO: climb up?
                } else /*if(!CanHangLeft && !CanHangRight)*/ {
                    // TODO: climb up?

                    Debug.LogWarning("Unexpectedly fell off!");
                    StopClimbing();

                    if(_breakOnFall) {
                        Debug.Break();
                    }
                }
                break;
            }
        }

#region Auto-Rotate/Climb
// TODO: encapsulate the common code from these

        private bool CheckWrapLeft()
        {
            if(null == _rightHandHitResult || PlayerBehavior.MoveDirection.x >= 0.0f) {
                return false;
            }

            _didWrapLeftRaycast = true;

            if(!Physics.Raycast(_leftHandTransform.position, Quaternion.AngleAxis(_data.WrapAroundAngle, transform.up) * transform.forward, out var hit, _data.ArmRayLength * 2.0f, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
            if(null == grabbable) {
                return false;
            }

            if(hit.normal == _rightHandHitResult.Value.normal) {
                return false;
            }

            _leftHandHitResult = hit;

            Vector3 offset = (Behavior.Owner.Radius * 2.0f) * transform.forward;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal) + offset, Quaternion.LookRotation(-hit.normal), _data.WrapTimeSeconds);

            return true;
        }

        private bool CheckRotateLeft()
        {
            if(null == _rightHandHitResult || PlayerBehavior.MoveDirection.x >= 0.0f) {
                return false;
            }

            _didRotateLeftRaycast = true;

            if(!Physics.Raycast(_leftHandTransform.position, Quaternion.AngleAxis(-90.0f, transform.up) * transform.forward, out var hit, _data.ArmRayLength * 0.5f, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
            if(null == grabbable) {
                return false;
            }

            if(hit.normal == _rightHandHitResult.Value.normal) {
                return false;
            }

            _leftHandHitResult = hit;

            Vector3 offset = (Behavior.Owner.Radius * 2.0f) * transform.forward;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal) - offset, Quaternion.LookRotation(-hit.normal), _data.WrapTimeSeconds);

            return true;
        }

        private bool CheckWrapRight()
        {
            if(null == _leftHandHitResult || PlayerBehavior.MoveDirection.x <= 0.0f) {
                return false;
            }

            _didWrapRightRaycast = true;

            if(!Physics.Raycast(_rightHandTransform.position, Quaternion.AngleAxis(-_data.WrapAroundAngle, transform.up) * transform.forward, out var hit, _data.ArmRayLength * 2.0f, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
            if(null == grabbable) {
                return false;
            }

            if(hit.normal == _leftHandHitResult.Value.normal) {
                return false;
            }

            _rightHandHitResult = hit;

            Vector3 offset = (Behavior.Owner.Radius * 2.0f) * transform.forward;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal) + offset, Quaternion.LookRotation(-hit.normal), _data.WrapTimeSeconds);

            return true;

        }

        private bool CheckRotateRight()
        {
            if(null == _leftHandHitResult || PlayerBehavior.MoveDirection.x <= 0.0f) {
                return false;
            }

            _didRotateRightRaycast = true;

            if(!Physics.Raycast(_rightHandTransform.position, Quaternion.AngleAxis(90.0f, transform.up) * transform.forward, out var hit, _data.ArmRayLength * 0.5f, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            IGrabbable grabbable = hit.transform.GetComponent<IGrabbable>();
            if(null == grabbable) {
                return false;
            }

            if(hit.normal == _leftHandHitResult.Value.normal) {
                return false;
            }

            _rightHandHitResult = hit;

            Vector3 offset = (Behavior.Owner.Radius * 2.0f) * transform.forward;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal) - offset, Quaternion.LookRotation(-hit.normal), _data.WrapTimeSeconds);

            return true;

        }

        private bool CheckClimbUp()
        {
            if(PlayerBehavior.MoveDirection.y <= 0.0f) {
                return false;
            }

            _didClimbUpRaycast = true;

            // cast a ray from the end of our rotated head check straight down to see if we can stand here
            Vector3 start = _headTransform.position + (Quaternion.AngleAxis(-_data.HeadRayAngle, transform.right) * transform.forward) * _data.HeadRayLength;
            float length = Behavior.Owner.Height;

            if(!Physics.Raycast(start, -Vector3.up, out var hit, length, _data.RaycastLayerMask, QueryTriggerInteraction.Ignore)) {
                return false;
            }

            Vector3 offset = Behavior.Owner.Radius * 2.0f * transform.forward;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, offset), Behavior.Owner.Movement.Rotation, _data.ClimbUpTimeSeconds, () => {
                StopClimbing();
            });

            return true;
        }

        private bool CheckHang()
        {
            if((!CanHangLeft && !CanHangRight) || PlayerBehavior.MoveDirection.y <= 0.0f) {
                return false;
            }

            RaycastHit hit = (_leftHandHangHitResult ?? _rightHandHangHitResult).Value;

            Vector3 offset = -_hangTransform.localPosition;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, offset), Behavior.Owner.Movement.Rotation, _data.HangTimeSeconds, () => {
                StartHanging();
            });

            return true;
        }

        private bool CheckClimb()
        {
            if((!CanGrabLeft && !CanGrabRight) || !Behavior.Owner.IsMoving) {
                return false;
            }

            RaycastHit hit = (_leftHandHitResult ?? _rightHandHitResult).Value;

            Vector3 offset = Behavior.Owner.Radius * transform.up;
            Behavior.Owner.ManualAnimator.StartAnimation(Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal) - offset, Behavior.Owner.Movement.Rotation, _data.ClimbDownTimeSeconds, () => {
                StartClimbing();
            });

            return true;
        }
#endregion

        private Vector3 GetSurfaceAttachmentPosition(RaycastHit hit, Vector3 offset)
        {
            // keep a set distance away from the surface
            Vector3 targetPoint = hit.point + (hit.normal * _data.AttachDistance);
            Vector3 a = targetPoint - Behavior.Owner.Movement.Position;
            Vector3 p = Vector3.Project(a, hit.normal);

            return p + offset;
        }

        private void AttachToSurface(RaycastHit hit, bool isHang)
        {
            Debug.Log($"Attach to surface {hit.transform.name}");

            // TODO: we can probably infer this using the dot product
            if(isHang) {
                Behavior.Owner.Movement.Position = Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, -_hangTransform.localPosition);
            } else {
                transform.forward = -hit.normal;
                Behavior.Owner.Movement.Position = Behavior.Owner.Movement.Position + GetSurfaceAttachmentPosition(hit, Behavior.Owner.Radius * hit.normal);
            }
        }

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => $"Character {Behavior.Owner.Id} Climbing Component");
            _debugMenuNode.RenderContentsAction = () => {
                _breakOnFall = GUILayout.Toggle(_breakOnFall, "Break on fall");
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
