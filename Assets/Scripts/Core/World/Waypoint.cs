using UnityEngine;

namespace pdxpartyparrot.Core.World
{
    public class Waypoint : MonoBehaviour
    {
#region Unity Lifecycle
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, 1);
        }
#endregion
    }
}
