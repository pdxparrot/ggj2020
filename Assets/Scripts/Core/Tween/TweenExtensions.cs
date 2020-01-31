#if USE_DOTWEEN
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public static class TweenExtensions
    {
        public static TweenerCore<Color, Color, ColorOptions> DOFade(this TextMeshProUGUI target, float endValue, float duration)
        {
            TweenerCore<Color, Color, ColorOptions> alpha = DOTween.ToAlpha(() => target.color, x => target.color = x, endValue, duration);
            alpha.SetTarget(target);
            return alpha;
        }
    }
}
#endif
