#if USE_DOTWEEN
using DG.Tweening;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenMove : TweenRunner
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
        private Vector3 _to;

        public Vector3 To
        {
            get => _to;
            set => _to = value;
        }

        [SerializeField]
        private bool _snapping;

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
                ? _target.DOLocalMove(_to, Duration, _snapping)
                : _target.DOMove(_to, Duration, _snapping);
        }
    }
}
#endif
