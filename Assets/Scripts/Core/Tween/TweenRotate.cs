#if USE_DOTWEEN
using DG.Tweening;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenRotate : TweenRunner
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
        private RotateMode _rotateMode = RotateMode.Fast;

        [SerializeField]
        private bool _useLocalRotation = true;

#region Unity Lifecycle
        protected override void Awake()
        {
            if(null == _target) {
                _target = transform;
            }

            _from = _useLocalRotation ? _target.localEulerAngles : _target.eulerAngles;

            base.Awake();
        }
#endregion

        public override void DoReset()
        {
            base.DoReset();

            if(_useLocalRotation) {
                _target.localEulerAngles = _from;
            } else {
                _target.eulerAngles = _from;
            }
        }

        protected override Tweener CreateTweener()
        {
            return _useLocalRotation
                ? _target.DOLocalRotate(_to, Duration, _rotateMode)
                : _target.DORotate(_to, Duration, _rotateMode);
        }
    }
}
#endif
