using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.World;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.World
{
    [RequireComponent(typeof(Collider))]
    public class Ladder : Actor3D, IGrabbable
    {
        public override bool IsLocalActor => true;
    }
}
