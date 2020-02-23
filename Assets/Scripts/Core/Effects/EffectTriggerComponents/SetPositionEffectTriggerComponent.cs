using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetPositionEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private Vector3 _position;

        [SerializeField]
        private bool _local;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            if(_local) {
                _gameObject.transform.localPosition = _position;
            } else {
                _gameObject.transform.position = _position;
            }
        }
    }
}
