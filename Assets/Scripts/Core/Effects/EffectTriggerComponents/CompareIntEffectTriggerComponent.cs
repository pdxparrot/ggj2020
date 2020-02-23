using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class CompareIntEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private int _value;

        [SerializeField]
        private EffectTrigger _true;

        [SerializeField]
        private EffectTrigger _false;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            int value = Owner.GetInt(_name);
            if(value == _value) {
                _true.Trigger();
            } else {
                _false.Trigger();
            }
        }
    }
}
