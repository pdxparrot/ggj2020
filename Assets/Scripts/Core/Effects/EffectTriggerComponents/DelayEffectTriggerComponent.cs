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

        public float Seconds
        {
            get => _seconds;
            set => _seconds = value;
        }

        // always wait for this to complete
        [SerializeField]
        [ReadOnly]
        private bool _waitForComplete = true;

        [SerializeField]
        [ReadOnly]
        private bool _isWaiting;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => !_isWaiting;

        public override void OnStart()
        {
            _isWaiting = true;
            TimeManager.Instance.RunAfterDelay(_seconds, () => _isWaiting = false);
        }
    }
}
