using System;

using pdxpartyparrot.Core.Actors.Components;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Players.Input;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public interface IPlayer
    {
        GameObject GameObject { get; }

        Guid Id { get; }

        bool IsLocalActor { get; }

        Game.Players.NetworkPlayer NetworkPlayer { get; }

        ActorBehaviorComponent Behavior { get; }

        ActorMovementComponent Movement { get; }

        PlayerBehavior PlayerBehavior { get; }

        PlayerInputHandler PlayerInputHandler { get; }

        Viewer Viewer { get; }
    }
}
