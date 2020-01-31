using JetBrains.Annotations;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    [RequireComponent(typeof(JumpBehaviorComponent))]
    public class LongJumpBehaviorComponent : CharacterBehaviorComponent
    {
        [SerializeField]
        private LongJumpBehaviorComponentData _data;

        [Space(10)]

#region Effects
        [Header("Effects")]

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _longJumpEffect;
#endregion

        [SerializeField]
        [ReadOnly]
        private bool _isHeld;

        [SerializeField]
        [ReadOnly]
        private float _heldSeconds;

        private bool CanLongJump => !_didLongJump && _heldSeconds >= _data.LongJumpHoldSeconds;

        [SerializeField]
        [ReadOnly]
        private bool _didLongJump;

#region Unity Lifecycle
        private void Update()
        {
            if(PartyParrotManager.Instance.IsPaused) {
                return;
            }

            float dt = Time.deltaTime;

            if(_isHeld) {
                _heldSeconds += dt;

                if(CanLongJump) {
                    Behavior.CharacterMovement.Jump(_data.LongJumpHeight);
                    if(null != _longJumpEffect) {
                        _longJumpEffect.Trigger();
                    }

                    _didLongJump = true;
                }
            }
        }

        private void LateUpdate()
        {
            if(!Behavior.IsGrounded) {
                _isHeld = false;
                _heldSeconds = 0;
            }
        }
#endregion

#region Actions
        public override bool OnStarted(CharacterBehaviorAction action)
        {
            if(!(action is JumpBehaviorComponent.JumpAction)) {
                return false;
            }

            if(!Behavior.IsGrounded || Behavior.IsSliding) {
                return false;
            }

            _isHeld = true;
            _heldSeconds = 0;
            _didLongJump = false;

            return true;
        }

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is JumpBehaviorComponent.JumpAction)) {
                return false;
            }

            _isHeld = false;
            _heldSeconds = 0;

            return _didLongJump;
        }
#endregion
    }
}
