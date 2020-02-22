using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public sealed class BackgroundRobot : Actor3D
    {
        public override bool IsLocalActor => true;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // TODO: this should come from an actor data object for this
            Rigidbody.isKinematic = true;
            Rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        }
#endregion
    }
}
