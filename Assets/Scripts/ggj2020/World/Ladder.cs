using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.Interactables;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.World
{
    [RequireComponent(typeof(Collider))]
    public sealed class Ladder : Actor3D, IInteractable
    {
        public override bool IsLocalActor => true;

        public bool CanInteract => true;

        public Type InteractableType => GetType();
    }
}
