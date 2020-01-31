using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private UnityEngine.Camera _camera;

        [CanBeNull]
        public UnityEngine.Camera Camera
        {
            get => _camera;
            set => _camera = value;
        }

        [SerializeField]
        [Tooltip("Allow rotation around the x-axis (vertical billboard)")]
        private bool _xRotation = true;

        [SerializeField]
        [Tooltip("Allow rotation around the y-axis (horizontal billboard)")]
        private bool _yRotation = true;

#region Unity Lifecycle
        private void LateUpdate()
        {
            if(null == Camera) {
                return;
            }

            Transform t = transform;

            // this is reversed to prevent the rotation flipping the y-axis
            Vector3 l = t.position - Camera.transform.position;

            if(!_xRotation) {
                l.y = 0.0f;
            }

            if(!_yRotation) {
                l.x = 0.0f;
            }

            t.rotation = Quaternion.LookRotation(l);
        }

        private void OnDrawGizmos()
        {
            Transform t = transform;
            Vector3 pos = t.position;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos, pos - t.forward * 2.0f);
        }
#endregion
    }
}
