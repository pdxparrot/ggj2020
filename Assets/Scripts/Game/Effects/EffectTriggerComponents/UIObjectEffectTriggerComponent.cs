using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.Effects.EffectTriggerComponents
{
    public class UIObjectEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _uiObjectName;

        [SerializeField]
        private bool _show;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            UIManager.Instance.ShowUIObject(_uiObjectName, _show);
        }
    }
}
