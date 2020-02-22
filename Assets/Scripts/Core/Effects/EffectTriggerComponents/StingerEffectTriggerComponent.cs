using pdxpartyparrot.Core.Audio;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class StingerEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private AudioClip _stinger;

        [SerializeField]
        private bool _waitForComplete;

        public override bool WaitForComplete => _waitForComplete;

        public override bool IsDone => !AudioManager.HasInstance || !AudioManager.Instance.IsStingerPlaying;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAudio) {
                AudioManager.Instance.PlayStinger(_stinger);
            }
        }

        public override void OnStop()
        {
            AudioManager.Instance.StopAllMusic();
        }
    }
}
