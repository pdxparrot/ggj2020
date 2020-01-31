using UnityEngine;

namespace pdxpartyparrot.Game.World
{
    [RequireComponent(typeof(Collider))]
    public class WorldBoundary3D : WorldBoundary
    {
#region Unity Lifecycle
        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCollision(other.gameObject);
        }
#endregion
    }
}