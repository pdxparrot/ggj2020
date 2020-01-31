using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Camera
{
    // http://2sa-studio.blogspot.com/2015/01/handling-aspect-ratio-in-unity2d.html
    [RequireComponent(typeof(UnityEngine.Camera))]
    public sealed class UICameraAspectRatio : MonoBehaviour
    {
        [SerializeField]
        private int _aspectWidth = 16;

        [SerializeField]
        private int _aspectHeight = 9;

        [SerializeField]
        [ReadOnly]
        private float _targetAspectRatio;

        private UnityEngine.Camera _camera;

#region Unity Lifecycle
        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            Assert.IsTrue(_camera.orthographic);

            _targetAspectRatio = _aspectWidth / (float)_aspectHeight;
        }

        private void Start()
        {
            UpdateAspectRatio();
        }
#endregion

        public void UpdateAspectRatio()
        {
            Rect cameraRect = _camera.rect;

            float viewportAspectRatio = (Screen.width * cameraRect.width) / (Screen.height * cameraRect.height);
            if(viewportAspectRatio >= _targetAspectRatio) {
                _camera.orthographicSize = _aspectHeight / 2.0f;
            }  else {
                float scale = _targetAspectRatio / viewportAspectRatio;
                _camera.orthographicSize = (_aspectHeight / 2.0f) * scale;
            }
            //Debug.Log($"Updated orthographic size of {_camera.name} to {_camera.orthographicSize}");
        }
    }
}
