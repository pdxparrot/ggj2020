using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetIntEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private int _value;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            Owner.SetInt(_name, _value);
        }
    }
}
