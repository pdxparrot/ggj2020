using UnityEngine;

namespace pdxpartyparrot.Game.World
{
    public abstract class WorldBoundary : MonoBehaviour
    {
        [SerializeField]
        private bool _deadly;

        public bool Deadly => _deadly;

        protected void HandleCollision(GameObject go)
        {
            IWorldBoundaryCollisionListener listener = go.GetComponent<IWorldBoundaryCollisionListener>();
            if(null == listener) {
                return;
            }

            listener.OnWorldBoundaryCollision(this);
        }
    }
}