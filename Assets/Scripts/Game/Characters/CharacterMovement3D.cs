using pdxpartyparrot.Core.Actors.Components;
using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters
{
    public class CharacterMovement3D : ActorMovementComponent3D, ICharacterMovement
    {
        public CharacterBehavior CharacterBehavior => (CharacterBehavior)Owner.Behavior;

        public override bool UseGravity
        {
            get => base.UseGravity;
            set
            {
                bool changed = base.UseGravity != value;
                base.UseGravity = value;
                if(changed && !value) {
                    Velocity = new Vector3(Velocity.x, 0.0f, Velocity.z);
                }
            }
        }

        [SerializeField]
        [ReadOnly]
        private bool _isComponentControlling;

        public bool IsComponentControlling { get; set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner.Behavior is CharacterBehavior);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            FudgeVelocity(dt);

            // turn off gravity if we're grounded and not moving and not sliding
            // this should stop us sliding down slopes we shouldn't slide down
            if(!IsComponentControlling) {
                UseGravity = !IsKinematic && (!CharacterBehavior.IsGrounded || Owner.IsMoving || CharacterBehavior.IsSliding);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Position, Position + AngularVelocity);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Position, Position + Velocity);
        }
#endregion

        protected override void InitRigidbody(ActorBehaviorComponentData behaviorData)
        {
            base.InitRigidbody(behaviorData);

            CharacterBehaviorData characterBehaviorData = behaviorData as CharacterBehaviorData;
            Assert.IsNotNull(characterBehaviorData);

            RigidBody.isKinematic = behaviorData.IsKinematic;
            RigidBody.useGravity = !behaviorData.IsKinematic;
            //RigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            RigidBody.detectCollisions = true;
            RigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public void EnableDynamicCollisionDetection(bool enable)
        {
            RigidBody.collisionDetectionMode = enable ? CollisionDetectionMode.ContinuousDynamic : CollisionDetectionMode.ContinuousSpeculative;
        }

        public override void PrepareJump()
        {
            base.PrepareJump();

            CharacterBehavior.IsGrounded = false;
        }

        public virtual void Jump(float height)
        {
            if(!CharacterBehavior.CanMove) {
                return;
            }

            PrepareJump();

            // factor in fall speed adjust
            float gravity = -Physics.gravity.y + CharacterBehavior.CharacterBehaviorData.FallSpeedAdjustment;

            // v = sqrt(2gh)
            Velocity = Vector3.up * Mathf.Sqrt(height * 2.0f * gravity);
        }

        private void FudgeVelocity(float dt)
        {
            Vector3 adjustedVelocity = Velocity;
            if(CharacterBehavior.IsGrounded && !Owner.IsMoving) {
                // prevent any weird ground adjustment shenanigans
                // when we're grounded and not moving
                adjustedVelocity.y = 0.0f;
            } else if(UseGravity) {
                // do some fudging to jumping/falling so it feels better
                float adjustment = CharacterBehavior.CharacterBehaviorData.FallSpeedAdjustment * dt;
                adjustedVelocity.y -= adjustment;

                // apply terminal velocity
                if(adjustedVelocity.y < -CharacterBehavior.CharacterBehaviorData.TerminalVelocity) {
                    adjustedVelocity.y = -CharacterBehavior.CharacterBehaviorData.TerminalVelocity;
                }
            }
            Velocity = adjustedVelocity;
        }
    }
}
