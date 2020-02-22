using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    public abstract class NPCBehavior : CharacterBehavior
    {
        public NPCBehaviorData NPCBehaviorData => (NPCBehaviorData)BehaviorData;

        public INPC NPCOwner => (INPC)Owner;

        public abstract Vector3 MoveDirection { get; }

#region Unity Lifecycle
        protected virtual void LateUpdate()
        {
            Owner.IsMoving = !Mathf.Approximately(MoveDirection.sqrMagnitude, 0.0f);
        }
#endregion

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(Owner is INPC);
            Assert.IsTrue(behaviorData is NPCBehaviorData);

            base.Initialize(behaviorData);
        }

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMove) {
                base.AnimationUpdate(dt);
                return;
            }

            Vector3 moveDirection = MoveDirection;

            AlignToMovement(moveDirection);

            if(null != Animator) {
                Animator.SetFloat(CharacterBehaviorData.MoveXAxisParam, CanMove ? Mathf.Abs(moveDirection.x) : 0.0f);
                Animator.SetFloat(CharacterBehaviorData.MoveZAxisParam, CanMove ? Mathf.Abs(moveDirection.y) : 0.0f);
            }

            base.AnimationUpdate(dt);
        }

        // TODO: this doesn't work with navmesh for some reason :(
        // turning off updatePosition causes it to never generate a path
        /*protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                base.PhysicsUpdate(dt);
                return;
            }

            if(!CharacterBehaviorData.AllowAirControl && IsFalling) {
                return;
            }

            Vector3 moveDirection = MoveDirection;

            Vector3 velocity = moveDirection * MoveSpeed;
            velocity = Owner.Movement.Rotation * velocity;

            if(Owner.Movement.IsKinematic) {
                Owner.Movement.Teleport(Owner.Movement.Position + velocity * dt);
            } else {
                velocity.y = Owner.Movement.Velocity.y;
                Owner.Movement.Velocity = velocity;
            }

            base.PhysicsUpdate(dt);
        }*/

#region Events
        public virtual void OnRecycle()
        {
        }
#endregion
    }
}
