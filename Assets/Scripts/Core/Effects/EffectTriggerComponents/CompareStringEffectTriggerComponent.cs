using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class CompareStringEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _name;

        [SerializeField]
        private string _value;

        [SerializeField]
        private bool _ignoreCase;

        [SerializeField]
        private EffectTrigger _true;

        [SerializeField]
        private EffectTrigger _false;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            string value = Owner.GetString(_name);
            if(value.Equals(value, _ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture)) {
                _true.Trigger();
            } else {
                _false.Trigger();
            }
        }
    }
}
