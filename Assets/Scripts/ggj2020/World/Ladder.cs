using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.Game.World;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.World
{
    [RequireComponent(typeof(Collider))]
    public class Ladder : Actor3D, IGrabbable, IInteractable
    {
        public override bool IsLocalActor => true;

        public bool CanInteract => true;
    }
}
