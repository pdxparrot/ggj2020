using UnityEngine;

namespace pdxpartyparrot.Core.Actors
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class Actor3D : Actor
    {
#region Collider
        public Rigidbody Rigidbody { get; private set; }

        public Collider Collider { get; private set; }
#endregion

        public override float Height => Collider.bounds.size.y;

        public override float Radius => Collider.bounds.size.x / 2.0f;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Rigidbody = GetComponent<Rigidbody>();
            Collider = GetComponent<Collider>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            RunOnComponents(c => c.CollisionEnter(collision.gameObject));
        }

        private void OnCollisionStay(Collision collision)
        {
            RunOnComponents(c => c.CollisionStay(collision.gameObject));
        }

        private void OnCollisionExit(Collision collision)
        {
            RunOnComponents(c => c.CollisionExit(collision.gameObject));
        }

        private void OnTriggerEnter(Collider other)
        {
            RunOnComponents(c => c.TriggerEnter(other.gameObject));
        }

        private void OnTriggerStay(Collider other)
        {
            RunOnComponents(c => c.TriggerStay(other.gameObject));
        }

        private void OnTriggerExit(Collider other)
        {
            RunOnComponents(c => c.TriggerExit(other.gameObject));
        }
#endregion
    }
}
