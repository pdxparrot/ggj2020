using System;
using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Collections;

using UnityEngine;

namespace pdxpartyparrot.Game.Interactables
{
    public abstract class Interactables : MonoBehaviour, IEnumerable<IInteractable>
    {
#region Events
        public event EventHandler<InteractableEventArgs> InteractableAddedEvent;
        public event EventHandler<InteractableEventArgs> InteractableRemovedEvent;
#endregion

        private readonly Dictionary<Type, HashSet<IInteractable>> _interactables = new Dictionary<Type, HashSet<IInteractable>>();

        public bool RemoveInteractable(IInteractable interactable)
        {
            var interactables = _interactables.GetOrAdd(interactable.GetType());
            return interactables.Remove(interactable);
        }

        public IReadOnlyCollection<IInteractable> GetInteractables<T>() where T: IInteractable
        {
            return _interactables.GetOrAdd(typeof(T));
        }

        public void GetInteractables<T>(ICollection<T> interactables) where T: class, IInteractable
        {
            IReadOnlyCollection<IInteractable> n = GetInteractables<T>();
            foreach(IInteractable interactable in n) {
                if(interactable is T scratch) {
                    interactables.Add(scratch);
                }
            }
        }

        public IEnumerator<IInteractable> GetEnumerator()
        {
            foreach(var kvp in _interactables) {
                foreach(IInteractable interactable in kvp.Value) {
                    yield return interactable;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool HasInteractables<T>() where T: IInteractable
        {
            var interactables = _interactables.GetOrDefault(typeof(T));
            return null != interactables && interactables.Count > 0;
        }

        public bool HasInteractable<T>(T interactable) where T: IInteractable
        {
            var interactables = _interactables.GetOrDefault(typeof(T));
            if(null == interactables) {
                return false;
            }
            return interactables.Contains(interactable);
        }

        protected void AddInteractable(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if(null == interactable || !interactable.CanInteract) {
                return;
            }

            var interactables = _interactables.GetOrAdd(interactable.GetType());
            if(interactables.Add(interactable)) {
                InteractableAddedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }

        protected void RemoveInteractable(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if(null == interactable) {
                return;
            }

            if(RemoveInteractable(interactable)) {
                InteractableRemovedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }
    }
}
