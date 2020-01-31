using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class ActivateGameObjectEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private GameObject _gameObject;

        [SerializeField]
        private bool _active;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _gameObject.SetActive(_active);
        }
    }
}
