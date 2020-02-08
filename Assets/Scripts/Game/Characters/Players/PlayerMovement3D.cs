using pdxpartyparrot.Core.Data.Actors.Components;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters
{
    public class PlayerMovement3D : CharacterMovement3D
    {
        protected override void InitRigidbody(ActorBehaviorComponentData behaviorData)
        {
            base.InitRigidbody(behaviorData);

            // we run the follow cam in FixedUpdate() and interpolation interferes with that
            RigidBody.interpolation = RigidbodyInterpolation.None;
        }
    }
}
