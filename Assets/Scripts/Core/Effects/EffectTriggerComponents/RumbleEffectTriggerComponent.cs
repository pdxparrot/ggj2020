using JetBrains.Annotations;

using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class RumbleEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private RumbleConfig _rumbleConfig;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private PlayerInputHelper _playerInput;

        public PlayerInputHelper PlayerInput
        {
            get => _playerInput;
            set => _playerInput = value;
        }

        [SerializeField]
        private bool _waitForComplete;

        public override bool WaitForComplete => _waitForComplete;

        [SerializeField]
        [ReadOnly]
        private bool _isPlaying;

        public override bool IsDone => !_isPlaying;

        public override void OnStart()
        {
            if(null != _playerInput && EffectsManager.Instance.EnableRumble) {
                _playerInput.Rumble(_rumbleConfig);
            }

            _isPlaying = true;

            // TODO: this won't respect pause in terms of duration and it really should
            // a coroutine that continually generates a short impulse over a large duration might make more sense
            TimeManager.Instance.RunAfterDelay(_rumbleConfig.Seconds, () => _isPlaying = false);
        }

        public override void OnStop()
        {
            // TODO: handle this
        }
    }
}
