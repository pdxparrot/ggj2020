using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    [RequireComponent(typeof(JumpBehaviorComponent))]
    public class HoverBehaviorComponent : CharacterBehaviorComponent
    {
#region Actions
        public class HoverAction : CharacterBehaviorAction
        {
            public static HoverAction Default = new HoverAction();
        }
#endregion

        [SerializeField]
        private HoverBehaviorComponentData _data;

        [SerializeField]
        [ReadOnly]
        private bool _isHeld;

        [SerializeField]
        [ReadOnly]
        private float _heldSeconds;

        private bool CanHover => _heldSeconds >= _data.HoverHoldSeconds;

        [SerializeField]
        [ReadOnly]
        private float _hoverTimeSeconds;

        public float RemainingPercent => 1.0f - (_hoverTimeSeconds / _data.HoverTimeSeconds);

        [SerializeReference]
        [ReadOnly]
        private ITimer _cooldownTimer;

        private bool IsHoverCooldown => _cooldownTimer.SecondsRemaining > 0.0f;

        [SerializeField]
        [ReadOnly]
        private bool _isHovering;

        public bool IsHovering => _isHovering;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _cooldownTimer = TimeManager.Instance.AddTimer();
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_cooldownTimer);
            }
            _cooldownTimer = null;

            base.OnDestroy();
        }

        private void Update()
        {
            if(PartyParrotManager.Instance.IsPaused) {
                return;
            }

            float dt = Time.deltaTime;

            if(_isHeld && !IsHoverCooldown) {
                _heldSeconds += dt;
            }

            if(IsHovering) {
                _hoverTimeSeconds += dt;
                if(_hoverTimeSeconds >= _data.HoverTimeSeconds) {
                    _hoverTimeSeconds = _data.HoverTimeSeconds;
                    StopHovering();
                }
            } else if(CanHover) {
                StartHovering();
            } else if(_hoverTimeSeconds > 0.0f) {
                _hoverTimeSeconds -= dt * _data.HoverRechargeRate;
                if(_hoverTimeSeconds < 0.0f) {
                    _hoverTimeSeconds = 0.0f;
                }
            }
        }

        private void LateUpdate()
        {
            if(Behavior.IsGrounded) {
                _isHeld = false;
                _heldSeconds = 0;

                StopHovering();
            }
        }
#endregion

        public override bool OnPhysicsUpdate(float dt)
        {
            if(!IsHovering) {
                return false;
            }

            Vector3 acceleration = (_data.HoverAcceleration + Behavior.CharacterBehaviorData.FallSpeedAdjustment) * Vector3.up;
            // TODO: this probably needs to be * or / mass since it's ForceMode.Force instead of ForceMode.Acceleration
            Behavior.Owner.Movement.AddForce(acceleration);

            return false;
        }

        public override bool OnStarted(CharacterBehaviorAction action)
        {
            if(!(action is HoverAction)) {
                return false;
            }

            if(Behavior.IsGrounded && !_data.HoverWhenGrounded) {
                return false;
            }

            _isHeld = true;
            _heldSeconds = 0;

            return true;
        }

        // NOTE: we want to consume jump actions if we're hovering
        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is JumpBehaviorComponent.JumpAction)) {
                return false;
            }

            return _isHovering;
        }

        public override bool OnCancelled(CharacterBehaviorAction action)
        {
            if(!(action is HoverAction)) {
                return false;
            }

            if(!IsHovering) {
                return false;
            }

            _isHeld = false;
            _heldSeconds = 0;

            bool wasHover = _isHovering;
            StopHovering();

            return wasHover;
        }

        private void StartHovering()
        {
            _isHovering = true;

            if(null != Behavior.Animator) {
                Behavior.Animator.SetBool(_data.HoverParam, true);
            }

            // stop all vertical movement immediately
            Behavior.Owner.Movement.Velocity = new Vector3(Behavior.Owner.Movement.Velocity.x, 0.0f, Behavior.Owner.Movement.Velocity.z);
        }

        public void StopHovering()
        {
            bool wasHovering = IsHovering;
            _isHovering = false;

            if(null != Behavior.Animator) {
                Behavior.Animator.SetBool(_data.HoverParam, false);
            }

            if(wasHovering) {
                _cooldownTimer.Start(_data.HoverCooldownSeconds);
            }
        }
    }
}
