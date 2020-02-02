#if USE_DOTWEEN
using DG.Tweening;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenScale : TweenRunner
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

#region Unity Lifecycle
        protected override void Awake()
        {
            if(null == _target) {
                _target = transform;
            }

            _from = _target.localScale;

            base.Awake();
        }
#endregion

        public override void DoReset()
        {
            base.DoReset();

            _target.localScale = _from;
        }

        protected override Tweener CreateTweener()
        {
            return _target.DOScale(_to, Duration);
        }
    }
}
#endif
