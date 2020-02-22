using JetBrains.Annotations;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class StopAudioEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        [CanBeNull]
        private AudioSource _audioSource;

        public AudioSource AudioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }

        public override bool WaitForComplete => false;

        public override bool IsDone => true;

        public override void OnStart()
        {
            _audioSource.Stop();
        }
    }
}
