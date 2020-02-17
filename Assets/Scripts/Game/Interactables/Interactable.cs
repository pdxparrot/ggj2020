using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        GameObject gameObject { get; }

        // this can return GetType() if that makes sense
        // but subclasses often need to return a common base class here
        Type InteractableType { get; }
    }
}
