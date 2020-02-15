using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetKinematicEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private bool _isKinematic;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _rigidbody.isKinematic = _isKinematic;
        }
    }
}
