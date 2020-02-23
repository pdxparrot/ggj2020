using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class CompareFloatEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private float _value;

        [SerializeField]
        private EffectTrigger _true;

        [SerializeField]
        private EffectTrigger _false;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            float value = Owner.GetFloat(_name);
            if(Mathf.Approximately(value, _value)) {
                _true.Trigger();
            } else {
                _false.Trigger();
            }
        }
    }
}
