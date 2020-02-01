using pdxpartyparrot.Core.Data.Actors.Components;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class PlayerFlightBehavior3D : PlayerBehavior
    {
        private CharacterFlightMovement3D CharacterFlightMovement3D => (CharacterFlightMovement3D)CharacterMovement;

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(Owner.Movement is CharacterFlightMovement3D);

            base.Initialize(behaviorData);
        }

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMove || null == Owner.Model) {
                base.AnimationUpdate(dt);
                return;
            }

            Transform modelTransform = Owner.Model.transform;

            Quaternion rotation = modelTransform.localRotation;
            Vector3 targetEuler = new Vector3
            {
                z = MoveDirection.x * -CharacterFlightMovement3D.FlightMovementData.MaxBankAngle,
                x = MoveDirection.y * -CharacterFlightMovement3D.FlightMovementData.MaxAttackAngle
            };

            Quaternion targetRotation = Quaternion.Euler(targetEuler);
            rotation = Quaternion.Lerp(rotation, targetRotation, CharacterFlightMovement3D.FlightMovementData.RotationAnimationSpeed * dt);

            modelTransform.localRotation = rotation;

            base.AnimationUpdate(dt);
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                base.PhysicsUpdate(dt);
                return;
            }

            CharacterFlightMovement3D.Turn(MoveDirection, dt);

            float attackAngle = MoveDirection.y * -CharacterFlightMovement3D.FlightMovementData.MaxAttackAngle;
            Vector3 attackVector = Quaternion.AngleAxis(attackAngle, Vector3.right) * Vector3.forward;
            CharacterFlightMovement3D.AddRelativeForce(attackVector * CharacterFlightMovement3D.FlightMovementData.LinearThrust, ForceMode.Force);

            // lift if we're not falling
            if(MoveDirection.y >= 0.0f) {
                CharacterFlightMovement3D.AddForce(-Physics.gravity, ForceMode.Acceleration);
            }

            // cap our fall speed
            Vector3 velocity = CharacterFlightMovement3D.Velocity;
            if(velocity.y < -CharacterFlightMovement3D.FlightMovementData.TerminalVelocity) {
                CharacterFlightMovement3D.Velocity = new Vector3(velocity.x, -CharacterFlightMovement3D.FlightMovementData.TerminalVelocity, velocity.z);
            }

            base.PhysicsUpdate(dt);
        }
    }
}
