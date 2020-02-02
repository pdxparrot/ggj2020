using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors.Components;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public abstract class Actor : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Guid _id;

        public Guid Id => _id;

        public abstract float Height { get; }

        public abstract float Radius { get; }

#region Model
        [SerializeField]
        [CanBeNull]
        private GameObject _model;

        [CanBeNull]
        public GameObject Model
        {
            get => _model;
            protected set => _model = value;
        }
#endregion

        [Space(10)]

#region Components
        [Header("Components")]

        [SerializeField]
        [ReorderableList]
        private ActorComponent.ReorderableList _components = new ActorComponent.ReorderableList();

#region Behavior
        [Header("Behavior")]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private ActorBehaviorComponent _behavior;

        [CanBeNull]
        public ActorBehaviorComponent Behavior => _behavior;
#endregion

#region Movement
        [Header("Movement")]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private ActorMovementComponent _movement;

        [CanBeNull]
        public ActorMovementComponent Movement => _movement;

        [SerializeField]
        [ReadOnly]
        private bool _isMoving;

        public bool IsMoving
        {
            get => _isMoving;

            set
            {
                bool wasMoving = _isMoving;
                _isMoving = value;

                if(wasMoving != _isMoving) {
                    OnMoveStateChanged();
                }
            }
        }

        [SerializeField]
        [ReadOnly]
        private Vector3 _facingDirection = new Vector3(1.0f, 0.0f, 0.0f);

        public Vector3 FacingDirection
        {
            get => _facingDirection;
            private  set => _facingDirection = value;
        }
#endregion

#region Animation
        [Header("Animation")]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private ActorManualAnimatorComponent _manualAnimator;

        [CanBeNull]
        public ActorManualAnimatorComponent ManualAnimator => _manualAnimator;
#endregion

#endregion

#region Network
        public abstract bool IsLocalActor { get; }
#endregion

#region Unity Lifecycle
        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            if(ActorManager.HasInstance) {
                ActorManager.Instance.Unregister(this);
            }
        }
#endregion

        public virtual void Initialize(Guid id)
        {
            if(ActorManager.Instance.EnableDebug) {
                Debug.Log($"Initializing actor {id}");
            }

            _id = id;
            name = Id.ToString();

            foreach(ActorComponent component in _components.Items) {
                component.Initialize(this);

                // cache some useful components while we're here
                switch(component)
                {
                case ActorBehaviorComponent behavior:
                    Assert.IsNull(_behavior);
                    _behavior = behavior;
                    break;
                case ActorMovementComponent movement:
                    Assert.IsNull(_movement);
                    _movement = movement;
                    break;
                }
            }
        }

#region Components
        [CanBeNull]
        public T GetActorComponent<T>() where T: ActorComponent
        {
            foreach(ActorComponent component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    return tc;
                }
            }
            return null;
        }

        public void GetActorComponents<T>(ICollection<T> components) where T: ActorComponent
        {
            components.Clear();
            foreach(ActorComponent component in _components.Items) {
                T tc = component as T;
                if(tc != null) {
                    components.Add(tc);
                }
            }
        }

        public void RunOnComponents(Func<ActorComponent, bool> f)
        {
            foreach(ActorComponent component in _components.Items) {
                if(f(component)) {
                    return;
                }
            }
        }
#endregion

        public void Think(float dt)
        {
            RunOnComponents(c => c.OnThink(dt));
        }

        public virtual void SetFacing(Vector3 direction)
        {
            if(direction.sqrMagnitude < MathUtil.Epsilon) {
                return;
            }

            FacingDirection = direction.normalized;

            RunOnComponents(c => c.OnSetFacing(FacingDirection));
        }

        public bool CanMove()
        {
            return CanMove(true);
        }

        public bool CanMove(bool components)
        {
            if(PartyParrotManager.Instance.IsPaused) {
                return false;
            }

            if(components) {
                foreach(ActorComponent component in _components.Items) {
                    if(!component.CanMove) {
                        return false;
                    }
                }
            }

            return true;
        }

        // TODO: would be better if we id radius (x) and height (y) separately
        public bool Collides(Actor other, float distance=float.Epsilon)
        {
            Vector3 offset = null != Movement && null != other.Movement
                            ? other.Movement.Position - Movement.Position
                            :  other.transform.position - transform.position;

            float r = other.Radius + Radius;
            float d = r * r;
            return offset.sqrMagnitude < d;
        }

        public void DeSpawn()
        {
            OnDeSpawn();

            gameObject.SetActive(false);
        }

#region Events
        public virtual bool OnSpawn(SpawnPoint spawnpoint)
        {
            ActorManager.Instance.Register(this);

            RunOnComponents(c => c.OnSpawn(spawnpoint));

            return true;
        }

        public virtual bool OnReSpawn(SpawnPoint spawnpoint)
        {
            ActorManager.Instance.Register(this);

            RunOnComponents(c => c.OnReSpawn(spawnpoint));

            return true;
        }

        public virtual void OnDeSpawn()
        {
            RunOnComponents(c => c.OnDeSpawn());

            if(ActorManager.HasInstance) {
                ActorManager.Instance.Unregister(this);
            }
        }

        protected virtual void OnMoveStateChanged()
        {
            RunOnComponents(c => c.OnMoveStateChanged());
        }
#endregion
    }
}
