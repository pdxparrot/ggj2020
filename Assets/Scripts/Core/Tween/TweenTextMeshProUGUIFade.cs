#if USE_DOTWEEN
using DG.Tweening;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenTextMeshProUGUIFade : TweenRunner
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private float _from;

        [SerializeField]
        private float _to = 1.0f;

        public override void DoReset()
        {
            base.DoReset();

            Color color = _text.color;
            color.a = _from;
            _text.color = color;
        }

        protected override Tweener CreateTweener()
        {
            return _text.DOFade(_to, Duration);
        }
    }
}
#endif
