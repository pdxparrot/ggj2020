using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors.Components
{
    public class ActorMovementComponent2D : ActorMovementComponent
    {
        [Serializable]
        protected struct InternalPauseState
        {
            public bool IsKinematic;
            public Vector3 Velocity;
            public float AngularVelocity;

            public void Save(Rigidbody2D rigidbody)
            {
                IsKinematic = rigidbody.isKinematic;
                rigidbody.isKinematic = true;

                Velocity = rigidbody.velocity;
                rigidbody.velocity = Vector3.zero;

                AngularVelocity = rigidbody.angularVelocity;
                rigidbody.angularVelocity = 0.0f;
            }

            public void Restore(Rigidbody2D rigidbody)
            {
                rigidbody.isKinematic = IsKinematic;
                rigidbody.velocity = Velocity;
                rigidbody.angularVelocity = AngularVelocity;
            }
        }

        [CanBeNull]
        protected Actor2D Owner2D => (Actor2D)Owner;

        [Space(10)]

#region Physics
        [Header("Physics")]

        // expose some useful rigidbody properties that unity doesn't
        [SerializeField]
        [ReadOnly]
        private float _lastGravityScale;

        // need the rigidbody hooked rather than coming from the owner
        // because we don't have the owner early enough
        [SerializeField]
        private Rigidbody2D _rigidbody;

        protected Rigidbody2D RigidBody => _rigidbody;

        public override Vector3 Position
        {
            get => _rigidbody.position;
            set
            {
                if(ActorManager.Instance.EnableDebug && null != Owner) {
                    Debug.Log($"Teleporting actor {Owner.Id} to {value}");
                }
                _rigidbody.position = value;
            }
        }

        public override Quaternion Rotation
        {
            get => Quaternion.AngleAxis(_rigidbody.rotation, Vector3.forward);
            set => _rigidbody.SetRotation(value);
        }

        public override Vector3 Velocity
        {
            get => _rigidbody.velocity;
            set => _rigidbody.velocity = value;
        }

        public float AngularVelocity
        {
            get => _rigidbody.angularVelocity;
            set => _rigidbody.angularVelocity = value;
        }

        public override float Mass
        {
            get => _rigidbody.mass;
            set => _rigidbody.mass = value;
        }

        public override float LinearDrag
        {
            get => _rigidbody.drag;
            set => _rigidbody.drag = value;
        }

        public override float AngularDrag
        {
            get => _rigidbody.angularDrag;
            set => _rigidbody.angularDrag = value;
        }

        public override bool IsKinematic
        {
            get => _rigidbody.isKinematic;
            set => _rigidbody.isKinematic = value;
        }

        public override bool UseGravity
        {
            get => _rigidbody.gravityScale > 0.0f;
            set => _rigidbody.gravityScale = value ? 1.0f : 0.0f;
        }
#endregion

        [Space(10)]

#region Pause State
        [Header("Pause State")]

        [SerializeField]
        [ReadOnly]
        private InternalPauseState _pauseState;

        protected InternalPauseState PauseState => _pauseState;
#endregion

#region Unity Lifecycle
        protected virtual void LateUpdate()
        {
            _lastGravityScale = _rigidbody.gravityScale;
        }
#endregion

        public override void Initialize(Actor owner)
        {
            Assert.IsTrue(owner is Actor2D);

            base.Initialize(owner);
        }

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            base.Initialize(behaviorData);

            InitRigidbody(behaviorData);
        }

        protected virtual void InitRigidbody(ActorBehaviorComponentData behaviorData)
        {
        }

        public override void ResetAngularVelocity()
        {
            AngularVelocity = 0.0f;
        }

        public override void Teleport(Vector3 position)
        {
            if(ActorManager.Instance.EnableDebug && null != Owner) {
                Debug.Log($"Teleporting actor {Owner.Id} to {position} (interpolated)");
            }
            _rigidbody.MovePosition(position);
        }

        public override void Move(Vector3 amount)
        {
            _rigidbody.MovePosition(Position + amount);
        }

        public override void MoveTowards(Vector3 position, float speed, float dt)
        {
            Vector3 newPosition = Vector3.MoveTowards(Position, position, speed * dt);
            _rigidbody.MovePosition(newPosition);
        }

        public override void MoveRotation(Quaternion rot)
        {
            _rigidbody.MoveRotation(rot);
        }

        public override void AddForce(Vector3 force)
        {
            AddForce(force, ForceMode2D.Force);
        }

        public override void AddImpulse(Vector3 force)
        {
            AddForce(force, ForceMode2D.Impulse);
        }

        public void AddForce(Vector3 force, ForceMode2D mode)
        {
            _rigidbody.AddForce(force, mode);
        }

        public void AddRelativeForce(Vector3 force, ForceMode2D mode)
        {
            _rigidbody.AddRelativeForce(force, mode);
        }

        public void AddTorque(float torque, ForceMode2D mode)
        {
            _rigidbody.AddTorque(torque, mode);
        }

#region Event Handlers
        protected override void PauseEventHandler(object sender, EventArgs args)
        {
            base.PauseEventHandler(sender, args);

            if(PartyParrotManager.Instance.IsPaused) {
                _pauseState.Save(_rigidbody);
            } else {
                _pauseState.Restore(_rigidbody);
            }
        }
#endregion
    }
}
