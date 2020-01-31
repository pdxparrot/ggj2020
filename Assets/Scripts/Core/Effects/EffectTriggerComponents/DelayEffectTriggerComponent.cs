using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    // this delay ignores pause
    // for a delay that doesn't ignore pause, use TimerEffectTriggerComponent
    public class DelayEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private float _seconds;

        [SerializeField]
        [ReadOnly]
        private bool _isWaiting;

        public override bool WaitForComplete => true;

        public override bool IsDone => !_isWaiting;

        public override void OnStart()
        {
            _isWaiting = true;
            TimeManager.Instance.RunAfterDelay(_seconds, () => _isWaiting = false);
        }
    }
}
