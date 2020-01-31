using pdxpartyparrot.Core.Audio;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class MusicEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private AudioClip _music;

        [SerializeField]
        [Tooltip("Immediately plays the music track if <= 0")]
        private float _transitionSeconds;

        public override bool WaitForComplete => false;

        public override bool IsDone => true;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAudio) {
                if(_transitionSeconds > 0.0f) {
                    AudioManager.Instance.TransitionMusicAsync(_music, _transitionSeconds);
                } else {
                    AudioManager.Instance.PlayMusic(_music);
                }
            }
        }

        public override void OnStop()
        {
            AudioManager.Instance.StopAllMusic();
        }
    }
}
