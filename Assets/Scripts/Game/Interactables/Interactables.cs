using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Interactables
{
    public abstract class Interactables : MonoBehaviour, IEnumerable<IInteractable>
    {
#region Events
        public event EventHandler<InteractableEventArgs> InteractableAddedEvent;
        public event EventHandler<InteractableEventArgs> InteractableRemovedEvent;
#endregion

        private readonly Dictionary<Type, HashSet<IInteractable>> _interactables = new Dictionary<Type, HashSet<IInteractable>>();

#region Unity Lifecycle
        protected virtual void Awake()
        {
            // hacky attempt at making sure we don't turn
            // an actor's non-trigger collider into a trigger
            Assert.IsNull(GetComponent<Actor>(), "Interactables modify their associated collider to be a trigger and should not be attached directly to an actor");
        }
#endregion

        // NOTE: this doesn't check to see if the interactable actually collides with us
        public bool AddInteractable(IInteractable interactable)
        {
            if(null == interactable || !interactable.CanInteract) {
                return false;
            }

            //Debug.Log($"Adding interactable of type {interactable.InteractableType}");

            var interactables = _interactables.GetOrAdd(interactable.InteractableType);
            return interactables.Add(interactable);
        }

        public bool RemoveInteractable(IInteractable interactable)
        {
            if(null == interactable) {
                return false;
            }

            //Debug.Log($"Removing interactable of type {interactable.InteractableType}");

            var interactables = _interactables.GetOrAdd(interactable.InteractableType);
            return interactables.Remove(interactable);
        }

        public IReadOnlyCollection<IInteractable> GetInteractables<T>() where T: IInteractable
        {
            return _interactables.GetOrAdd(typeof(T));
        }

        [CanBeNull]
        public T GetRandomInteractable<T>() where T: class, IInteractable
        {
            return  GetInteractables<T>().GetRandomEntry() as T;
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
            if(AddInteractable(interactable)) {
                InteractableAddedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }

        protected void RemoveInteractable(GameObject other)
        {
            IInteractable interactable = other.GetComponent<IInteractable>();
            if(RemoveInteractable(interactable)) {
                InteractableRemovedEvent?.Invoke(this, new InteractableEventArgs{
                    Interactable = interactable
                });
            }
        }
    }
}
