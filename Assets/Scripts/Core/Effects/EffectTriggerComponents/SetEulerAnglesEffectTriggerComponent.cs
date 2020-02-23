using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetEulerAnglesEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private Vector3 _eulerAngles;

        [SerializeField]
        private bool _local;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            if(_local) {
                _gameObject.transform.localEulerAngles = _eulerAngles;
            } else {
                _gameObject.transform.eulerAngles = _eulerAngles;
            }
        }
    }
}
