#if USE_SPINE
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SpineSkinFadeEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private SpineSkinHelper _skinHelper;

        [SerializeField]
        private Color _fadeTo = new Color(0.0f, 0.0f, 0.0f, 1.0f);

        [SerializeField]
        private float _transitionTime = 1.0f;

        [SerializeField]
        [ReadOnly]
        private Color _difference;

        [SerializeField]
        [ReadOnly]
        private float _timeRemaining;

        public override bool WaitForComplete => true;

        public override bool IsDone => _timeRemaining <= 0.0f;

        public override void OnStart()
        {
            _timeRemaining = _transitionTime;
            _difference = new Color(Mathf.Abs(_fadeTo.r - _skinHelper.Color.r),
                                    Mathf.Abs(_fadeTo.g - _skinHelper.Color.g),
                                    Mathf.Abs(_fadeTo.b - _skinHelper.Color.b),
                                    Mathf.Abs(_fadeTo.a - _skinHelper.Color.a));
        }

        public override void OnStop()
        {
            _skinHelper.Color = _fadeTo;
            _timeRemaining = 0.0f;
        }

        public override void OnUpdate(float dt)
        {
            float pct = Mathf.Clamp01(dt / _transitionTime);
            Color step = pct * _difference;

            Color currentColor = _skinHelper.Color;
            Color newColor = new Color(Mathf.MoveTowards(currentColor.r, _fadeTo.r, step.r),
                                       Mathf.MoveTowards(currentColor.g, _fadeTo.g, step.g),
                                       Mathf.MoveTowards(currentColor.b, _fadeTo.b, step.b),
                                       Mathf.MoveTowards(currentColor.a, _fadeTo.a, step.a));
            _skinHelper.Color = newColor;

            _timeRemaining -= dt;
            if(_timeRemaining < 0.0f) {
                _skinHelper.Color = _fadeTo;
                _timeRemaining = 0.0f;
            }
        }
    }
}
#endif
