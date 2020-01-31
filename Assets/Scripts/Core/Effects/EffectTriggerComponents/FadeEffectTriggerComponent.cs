using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class FadeEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Image _image;

        public Image Image
        {
            get => _image;
            set => _image = value;
        }

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
            _difference = new Color(Mathf.Abs(_fadeTo.r - Image.color.r),
                                    Mathf.Abs(_fadeTo.g - Image.color.g),
                                    Mathf.Abs(_fadeTo.b - Image.color.b),
                                    Mathf.Abs(_fadeTo.a - Image.color.a));
        }

        public override void OnStop()
        {
            Image.color = _fadeTo;
            _timeRemaining = 0.0f;
        }

        public override void OnUpdate(float dt)
        {
            float pct = Mathf.Clamp01(dt / _transitionTime);
            Color step = pct * _difference;

            Color currentColor = Image.color;
            Color newColor = new Color(Mathf.MoveTowards(currentColor.r, _fadeTo.r, step.r),
                                       Mathf.MoveTowards(currentColor.g, _fadeTo.g, step.g),
                                       Mathf.MoveTowards(currentColor.b, _fadeTo.b, step.b),
                                       Mathf.MoveTowards(currentColor.a, _fadeTo.a, step.a));
            Image.color = newColor;

            _timeRemaining -= dt;
            if(_timeRemaining < 0.0f) {
                Image.color = _fadeTo;
                _timeRemaining = 0.0f;
            }
        }
    }
}
