#if USE_DOTWEEN
using pdxpartyparrot.Core.Tween;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class TweenEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private TweenSequence _tweenSequence;

        [SerializeField]
        private bool _waitForComplete;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => !_tweenSequence.IsRunning;

        public override void OnStart()
        {
            _tweenSequence.Play();
        }

        public override void OnStop()
        {
            _tweenSequence.Complete(true);
        }

        public float GetDuration()
        {
            return _tweenSequence.GetDuration();
        }
    }
}
#endif
