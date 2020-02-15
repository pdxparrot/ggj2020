using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetUseGravityEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private bool _useGravity;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _rigidbody.useGravity = _useGravity;
        }
    }
}
