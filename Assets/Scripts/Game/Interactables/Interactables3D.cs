using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    [RequireComponent(typeof(Collider))]
    public class Interactables3D : Interactables
    {
        private Collider _trigger;

#region Unity Life Cycle
        protected override void Awake()
        {
            base.Awake();

            _trigger = GetComponent<Collider>();
            _trigger.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            AddInteractable(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            RemoveInteractable(other.gameObject);
        }
#endregion
    }
}
