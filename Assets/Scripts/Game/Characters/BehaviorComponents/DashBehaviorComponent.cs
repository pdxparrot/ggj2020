using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    // TODO: make air dashing configurable
    // TODO: make it possible to perform a dash action
    // without invoking the cooldown (action parameter)
    public class DashBehaviorComponent : CharacterBehaviorComponent
    {
#region Actions
        public class DashAction : CharacterBehaviorAction
        {
            public static DashAction Default = new DashAction();
        }
#endregion

        [SerializeField]
        private DashBehaviorComponentData _data;

        public DashBehaviorComponentData DashBehaviorComponentData
        {
            get => _data;
            set => _data = value;
        }

#region Effects
        [Header("Effects")]

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _dashEffect;
#endregion

        [SerializeReference]
        [ReadOnly]
        private ITimer _dashTimer;

        public bool IsDashing => _dashTimer.IsRunning;

        [SerializeReference]
        [ReadOnly]
        private ITimer _cooldownTimer;

        public bool IsDashCooldown => _cooldownTimer.IsRunning;

        public bool CanDash => !IsDashing && !IsDashCooldown;

        [SerializeField]
        [ReadOnly]
        private bool _wasUseGravity;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _dashTimer = TimeManager.Instance.AddTimer();
            _dashTimer.StopEvent += DashStopEventHandler;
            _dashTimer.TimesUpEvent += DashTimesUpEventHandler;

            _cooldownTimer = TimeManager.Instance.AddTimer();
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_dashTimer);
                TimeManager.Instance.RemoveTimer(_cooldownTimer);
            }

            _dashTimer = null;
            _cooldownTimer = null;

            base.OnDestroy();
        }
#endregion

        public override bool OnPhysicsUpdate(float dt)
        {
            if(!IsDashing) {
                return false;
            }

            Vector3 moveDirection = Behavior.Owner.FacingDirection;
            Vector3 velocity = moveDirection * DashBehaviorComponentData.DashSpeed;

            Behavior.Owner.Movement.Move(velocity * dt);

            return true;
        }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is DashAction)) {
                return false;
            }

            StartDashing();

            return true;
        }

        private void StartDashing()
        {
            Behavior.CharacterMovement.IsComponentControlling = true;

            if(DashBehaviorComponentData.DisableGravity) {
                _wasUseGravity = Behavior.Owner.Movement.UseGravity;
                Behavior.Owner.Movement.UseGravity = false;
            }
            Behavior.CharacterMovement.EnableDynamicCollisionDetection(true);

            _dashTimer.Start(DashBehaviorComponentData.DashTimeSeconds);

            if(null != _dashEffect) {
                _dashEffect.Trigger();
            }
        }

        private void StopDashing()
        {
            _cooldownTimer.Start(DashBehaviorComponentData.DashCooldownSeconds);

            Behavior.CharacterMovement.EnableDynamicCollisionDetection(false);

            if(DashBehaviorComponentData.DisableGravity) {
                Behavior.Owner.Movement.UseGravity = _wasUseGravity;
            }

            Behavior.CharacterMovement.IsComponentControlling = false;
        }

#region Event Handlers
        protected virtual void DashStopEventHandler(object sender, EventArgs args)
        {
            StopDashing();
        }

        protected virtual void DashTimesUpEventHandler(object sender, EventArgs args)
        {
            StopDashing();
        }
#endregion
    }
}
