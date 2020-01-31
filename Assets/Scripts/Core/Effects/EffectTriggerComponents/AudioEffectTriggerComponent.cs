using JetBrains.Annotations;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Time;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class AudioEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        [CanBeNull]
        private AudioClip _audioClip;

        public AudioClip AudioClip
        {
            get => _audioClip;
            set => _audioClip = value;
        }

        [SerializeField]
        [CanBeNull]
        private AudioSource _audioSource;

        public AudioSource AudioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }

        [SerializeField]
        private bool _waitForComplete;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => (null == _audioSource || !_audioSource.isPlaying) && !_audioTimer.IsRunning;

        private ITimer _audioTimer;

#region Unity Lifecycle
        private void Awake()
        {
            _audioTimer = TimeManager.Instance.AddTimer();
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_audioTimer);
            }
        }
#endregion

        public override void Initialize(EffectTrigger owner)
        {
            base.Initialize(owner);

            if(null != _audioSource) {
                AudioManager.Instance.InitSFXAudioMixerGroup(_audioSource);
            }
        }

        public override void OnStart()
        {
            if(null == _audioClip) {
                return;
            }

            if(EffectsManager.Instance.EnableAudio) {
                if(null == _audioSource) {
                    AudioManager.Instance.PlayOneShot(_audioClip);
                    _audioTimer.Start(_audioClip.length);
                } else {
                    _audioSource.clip = _audioClip;
                    _audioSource.Play();
                }
            } else {
                _audioTimer.Start(_audioClip.length);
            }
        }

        public override void OnStop()
        {
            if(null != _audioSource) {
                _audioSource.Stop();
            }
            _audioTimer.Stop();
        }
    }
}
