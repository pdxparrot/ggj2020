using System;

using pdxpartyparrot.Core.Data.Actors.Components;

using UnityEngine;

namespace pdxpartyparrot.Core.Actors.Components
{
    // TODO: this should be capable of moving an actor (2D or 3D)
    // without relying on Unity's physics system (eg. no Rigidbody)
    public class ActorMovementComponent : ActorComponent
    {
        //[Space(10)]

#region Physics
        //[Header("Physics")]

        private Transform _transform;

        public virtual Vector3 Position
        {
            get => _transform.position;
            set
            {
                if(ActorManager.Instance.EnableDebug && null != Owner) {
                    Debug.Log($"Teleporting actor {Owner.Id} to {value}");
                }
                _transform.position = value;
            }
        }

        public virtual Quaternion Rotation
        {
            get => _transform.rotation;
            set => _transform.rotation = value;
        }

        public virtual Vector3 Velocity
        {
            get => Vector3.zero;
            set {}
        }

        public virtual float Mass
        {
            get => 1.0f;
            set {}
        }

        public virtual float LinearDrag
        {
            get => 0.0f;
            set {}
        }

        public virtual float AngularDrag
        {
            get => 0.0f;
            set {}
        }

        public virtual bool IsKinematic
        {
            get => true;
            set {}
        }

        public virtual bool UseGravity
        {
            get => false;
            set {}
        }
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            // always start out kinematic so that we don't
            // fall while we're loading
            IsKinematic = true;

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        protected override void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }

            base.OnDestroy();
        }
#endregion

        public override void Initialize(Actor owner)
        {
            base.Initialize(owner);

            _transform = owner.GetComponent<Transform>();
        }

        public virtual void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Rotation = Quaternion.identity;
            Velocity = Vector3.zero;
            ResetAngularVelocity();
        }

        protected virtual void ResetFromData(ActorBehaviorComponentData behaviorData)
        {
            Mass = behaviorData.Mass;
            LinearDrag = behaviorData.Drag;
            AngularDrag = behaviorData.AngularDrag;
            IsKinematic = behaviorData.IsKinematic;
            UseGravity = !behaviorData.IsKinematic;
        }

        public virtual void ResetAngularVelocity()
        {
            // TODO
        }

        // force physics to a sane state for the first frame of the jump
        // this can also be used to prepare for other y-direction velocity changes
        public virtual void PrepareJump()
        {
            UseGravity = true;
        }

        public virtual void Teleport(Vector3 position)
        {
            if(ActorManager.Instance.EnableDebug && null != Owner) {
                Debug.Log($"Teleporting actor {Owner.Id} to {position}");
            }
            _transform.position = position;
        }

        public virtual void Move(Vector3 amount)
        {
            _transform.position = Position + amount;
        }

        public virtual void MoveTowards(Vector3 position, float speed, float dt)
        {
            Vector3 newPosition = Vector3.MoveTowards(Position, position, speed * dt);
            _transform.position = newPosition;
        }

        public virtual void MoveRotation(Quaternion rot)
        {
            _transform.rotation = rot;
        }

        public virtual void AddForce(Vector3 force)
        {
            // TODO
        }

        public virtual void AddImpulse(Vector3 force)
        {
            // TODO
        }

#region Event Handlers
        protected virtual void PauseEventHandler(object sender, EventArgs args)
        {
        }
#endregion
    }
}
