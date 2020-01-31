using Cinemachine;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Camera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    [RequireComponent(typeof(CinemachineImpulseListener))]
    public class CinemachineViewer : Viewer
    {
        private enum Mode 
        {
            Mode2D,
            Mode3D,
        }

        [SerializeField]
        [ReadOnly]
        private Mode _mode = Mode.Mode3D;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private CinemachineVirtualCamera _virtualCamera;

        private CinemachineBrain _cinemachineBrain;

        [CanBeNull]
        private CinemachineImpulseListener _impulseListener;

        [CanBeNull]
        protected CinemachineImpulseListener ImpulseListener => _impulseListener;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _impulseListener = GetComponent<CinemachineImpulseListener>();

            _cinemachineBrain = Camera.GetComponent<CinemachineBrain>();
            Assert.IsNotNull(_cinemachineBrain);
        }
#endregion

        public void SetVirtualCamera(CinemachineVirtualCamera virtualCamera)
        {
            _virtualCamera = virtualCamera;
            switch(_mode)
            {
            case Mode.Mode2D:
                Set2D();
                break;
            case Mode.Mode3D:
                Set3D();
                break;
            }
        }

        public override void Set2D(float size)
        {
            base.Set2D(size);

            if(null != _virtualCamera) {
                _virtualCamera.m_Lens.OrthographicSize = size;
            }

            if(null != _impulseListener) {
                _impulseListener.m_Use2DDistance = true;
            }
        }

        public override void Set3D(float fieldOfView)
        {
            base.Set3D(fieldOfView);

            if(null != _virtualCamera) {
                _virtualCamera.m_Lens.FieldOfView = fieldOfView;
            }

            if(null != _impulseListener) {
                _impulseListener.m_Use2DDistance = false;
            }
        }

        [CanBeNull]
        public T GetCinemachineComponent<T>() where T: CinemachineComponentBase
        {
            return null != _virtualCamera ? _virtualCamera.GetCinemachineComponent<T>() : null;
        }

        public void Follow(Transform target)
        {
            if(null != _virtualCamera) {
                _virtualCamera.Follow = target;
            }
        }

        public void LookAt(Transform target)
        {
            if(null != _virtualCamera) {
                _virtualCamera.LookAt = target;
            }
        }
    }
}