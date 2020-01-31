#if USE_DOTWEEN
using DG.Tweening;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenImageFade : TweenRunner
    {
        [SerializeField]
        private Image _image;

        [SerializeField]
        private float _from;

        [SerializeField]
        private float _to = 1.0f;

        public override void DoReset()
        {
            base.DoReset();

            Color color = _image.color;
            color.a = _from;
            _image.color = color;
        }

        protected override Tweener CreateTweener()
        {
            return _image.material.DOFade(_to, Duration);
        }
    }
}
#endif
