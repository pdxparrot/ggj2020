using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetFloatEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private float _value;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            Owner.SetFloat(_name, _value);
        }
    }
}
