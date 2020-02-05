using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    [RequireComponent(typeof(Collider2D))]
    public class Interactables2D : Interactables
    {
        private Collider2D _trigger;

#region Unity Life Cycle
        protected override void Awake()
        {
            base.Awake();

            _trigger = GetComponent<Collider2D>();
            _trigger.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            AddInteractable(other.gameObject);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            RemoveInteractable(other.gameObject);
        }
#endregion
    }
}
