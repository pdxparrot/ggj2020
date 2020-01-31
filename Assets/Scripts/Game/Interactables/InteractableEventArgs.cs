using System;

namespace pdxpartyparrot.Game.Interactables
{
    public class InteractableEventArgs : EventArgs
    {
        public IInteractable Interactable { get; set; }
    }
}
