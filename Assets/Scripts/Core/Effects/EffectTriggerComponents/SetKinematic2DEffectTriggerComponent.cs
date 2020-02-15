using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetKinematic2DEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private bool _isKinematic;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _rigidbody.isKinematic = _isKinematic;
        }
    }
}
