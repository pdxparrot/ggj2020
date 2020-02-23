using JetBrains.Annotations;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    // TODO: rename SFXEffectTriggerComponent
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
        private bool _loop;

        [SerializeField]
        private bool _waitForComplete;

        // don't wait for complete if the audio should loop
        public override bool WaitForComplete => !_loop && _waitForComplete;

        public override bool IsDone => (null == _audioSource || !_audioSource.isPlaying) && (null == _audioTimer || !_audioTimer.IsRunning);

        [SerializeReference]
        [ReadOnly]
        [CanBeNull]
        private ITimer _audioTimer;

#region Unity Lifecycle
        private void Awake()
        {
            if(_loop) {
                Assert.IsFalse(_waitForComplete);
                Assert.IsNotNull(_audioSource);
            } else {
                _audioTimer = TimeManager.Instance.AddTimer();
            }
        }

        private void OnDestroy()
        {
            if(null != _audioTimer && TimeManager.HasInstance) {
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
                    if(null != _audioTimer) {
                        _audioTimer.Start(_audioClip.length);
                    }
                } else {
                    _audioSource.clip = _audioClip;
                    _audioSource.loop = _loop;
                    _audioSource.Play();
                }
            } else if(null != _audioTimer) {
                _audioTimer.Start(_audioClip.length);
            }
        }

        public override void OnStop()
        {
            if(null != _audioSource) {
                _audioSource.Stop();
            }

            if(null != _audioTimer) {
                _audioTimer.Stop();
            }
        }
    }
}
