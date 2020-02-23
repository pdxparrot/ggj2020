using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextBlink : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The time, in seconds, to complete a full blink cycle")]
        private float _blinkRate = 0.5f;

        [SerializeField]
        [Tooltip("Delay between blink states")]
        private float _delay = 0.5f;

        [SerializeField]
        [Tooltip("Fade in and out")]
        private bool _fade;

        [SerializeField]
        private bool _startOnAwake;

        [SerializeField]
        [ReadOnly]
        private bool _blinkOut = true;

        [SerializeReference]
        [ReadOnly]
        private ITimer _blinkTimer;

        [SerializeReference]
        [ReadOnly]
        private ITimer _delayTimer;

        private TextMeshProUGUI _text;

#region Unity Lifecycle
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            _blinkTimer = TimeManager.Instance.AddTimer();
            _blinkTimer.TimesUpEvent += BlinkTimesUpEventHandler;

            _delayTimer = TimeManager.Instance.AddTimer();
            _delayTimer.TimesUpEvent += DelayTimesUpEventHandler;

            if(_startOnAwake) {
                StartBlink();
            }
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_blinkTimer);
                _blinkTimer = null;

                TimeManager.Instance.RemoveTimer(_delayTimer);
                _delayTimer = null;
            }
        }
#endregion

        public void StartBlink()
        {
            StopBlink();

            _delayTimer.Start(_delay);
        }

        public void StopBlink()
        {
            if(null == _delayTimer || null == _blinkTimer) {
                return;
            }

            _delayTimer.Stop();
            _blinkTimer.Stop();

            if(_fade) {
                _text.CrossFadeAlpha(1.0f, 0.0f, true);
            } else {
                _text.alpha = 1.0f;
            }
        }

        private void DoBlink(bool blinkOut)
        {
            _blinkOut = blinkOut;

            float duration = _blinkRate * 0.5f;

            float target = _blinkOut ? 0.0f : 1.0f;
            if(_fade) {
                _text.CrossFadeAlpha(target, duration, true);
            } else {
                _text.alpha = target;
            }
            _blinkTimer.Start(duration);
        }

#region Event Handlers
        private void BlinkTimesUpEventHandler(object sender, EventArgs args)
        {
            if(_blinkOut) {
                DoBlink(false);
            } else {
                _delayTimer.Start(_delay);
            }
        }

        private void DelayTimesUpEventHandler(object sender, EventArgs args)
        {
            DoBlink(true);
        }
#endregion
    }
}
