using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class AnimationEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private string _animationTriggerParameter;

        // TODO: if we can know when the animation will finish, we can set this
        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            if(EffectsManager.Instance.EnableAnimation) {
                _animator.SetTrigger(_animationTriggerParameter);
            }

            // TODO: is there any way to know how long the triggered animation will take?
        }

        public override void OnStop()
        {
            // TODO: handle this
        }
    }
}
