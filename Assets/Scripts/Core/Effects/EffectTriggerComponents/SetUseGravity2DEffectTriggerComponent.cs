using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetUseGravity2DEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private Rigidbody2D _rigidbody;

        [SerializeField]
        private bool _useGravity;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _rigidbody.gravityScale = _useGravity ? 1.0f : 0.0f;
        }
    }
}
