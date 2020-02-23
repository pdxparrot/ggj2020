using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SetScaleEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private Vector3 _scale;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _gameObject.transform.localScale = _scale;
        }
    }
}
