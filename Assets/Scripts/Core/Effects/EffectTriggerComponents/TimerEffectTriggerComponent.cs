using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class TimerEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private float _seconds;

        // always wait for this to complete
        [SerializeField]
        [ReadOnly]
        private bool _waitForComplete = true;

        [SerializeReference]
        [ReadOnly]
        private ITimer _timer;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => !_timer.IsRunning;

#region Unity Lifecycle
        private void Awake()
        {
            _timer = TimeManager.Instance.AddTimer();
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_timer);
            }
            _timer = null;
        }
#endregion

        public override void OnStart()
        {
            _timer.Start(_seconds);
        }

        public override void OnStop()
        {
            _timer.Stop();
        }
    }
}
