using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetRotationEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private Quaternion _rotation;

        [SerializeField]
        private bool _local;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            if(_local) {
                _gameObject.transform.rotation = _rotation;
            } else {
                _gameObject.transform.rotation = _rotation;
            }
        }
    }
}
