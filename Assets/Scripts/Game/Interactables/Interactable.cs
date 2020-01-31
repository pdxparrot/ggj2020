using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    public interface IInteractable
    {
        bool CanInteract { get; }

        GameObject gameObject { get; }
    }
}
