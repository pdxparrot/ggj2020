using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetStringEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private string _value;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            Owner.SetString(_name, _value);
        }
    }
}
