#if USE_DOTWEEN
using System.Collections.Generic;
using System.Linq;

using DG.Tweening;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenPath : TweenRunner
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        [ReadOnly]
        private Vector3 _from;

        public Vector3 From
        {
            get => _from;
            set => _from = value;
        }

        [SerializeField]
        private Vector3[] _waypoints;

        public IReadOnlyCollection<Vector3> Waypoints
        {
            get => _waypoints;
            set => _waypoints = value.ToArray();
        }

        [SerializeField]
        private PathType _pathType = PathType.Linear;

        [SerializeField]
        private PathMode _pathMode = PathMode.Full3D;

        [SerializeField]
        private int _resolution = 10;

        [SerializeField]
        private Color? _gizmoColor;

        [SerializeField]
        private bool _useLocalPosition = true;

#region Unity Lifecycle
        protected override void Awake()
        {
            if(null == _target) {
                _target = transform;
            }

            _from = _useLocalPosition ? _target.localPosition : _target.position;

            base.Awake();
        }
#endregion

        public override void DoReset()
        {
            base.DoReset();

            if(_useLocalPosition) {
                _target.localPosition = _from;
            } else {
                _target.position = _from;
            }
        }

        protected override Tweener CreateTweener()
        {
            return _useLocalPosition
                ? _target.DOLocalPath(_waypoints, Duration, _pathType, _pathMode, _resolution, _gizmoColor)
                : _target.DOPath(_waypoints, Duration, _pathType, _pathMode, _resolution, _gizmoColor);
        }
    }
}
#endif
