using pdxpartyparrot.Core.Audio;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class StopMusicEffectTriggerComponent : EffectTriggerComponent
    {
        public override bool WaitForComplete => false;

        public override bool IsDone => true;

        public override void OnStart()
        {
            AudioManager.Instance.StopAllMusic();
        }
    }
}
