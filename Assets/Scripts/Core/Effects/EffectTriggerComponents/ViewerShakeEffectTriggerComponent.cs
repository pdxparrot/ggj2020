#if USE_DOTWEEN
using DG.Tweening;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class ViewerShakeEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        [ReadOnly]
        private Viewer _viewer;

        public Viewer Viewer
        {
            get => _viewer;
            set => _viewer = value;
        }

        [SerializeField]
        private ShakeConfig _screenShakeConfig;

        [SerializeField]
        private bool _waitForComplete = true;

        public override bool WaitForComplete => _waitForComplete;

        [SerializeField]
        [ReadOnly]
        private bool _isPlaying;

        public override bool IsDone => !_isPlaying;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableViewerShake) {
                Viewer.Camera.DOShakePosition(_screenShakeConfig.Duration, _screenShakeConfig.Strength, _screenShakeConfig.Vibrato, _screenShakeConfig.Randomness, _screenShakeConfig.FadeOut);
            }

            _isPlaying = true;

            // TODO: this won't respect pause in terms of duration and it really should
            // a coroutine that continually generates a short impulse over a large duration might make more sense
            TimeManager.Instance.RunAfterDelay(_screenShakeConfig.Duration, () => _isPlaying = false);
        }

        public override void OnStop()
        {
            // TODO: handle this
        }
    }
}
#endif
