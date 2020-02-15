using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.Interactables;

namespace pdxpartyparrot.ggj2020.World
{
    public sealed class Ladder : Actor3D, IInteractable
    {
        public override bool IsLocalActor => true;

        public bool CanInteract => true;

        public Type InteractableType => GetType();

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Rigidbody.isKinematic = true;
            Collider.isTrigger = true;
        }
#endregion
    }
}
