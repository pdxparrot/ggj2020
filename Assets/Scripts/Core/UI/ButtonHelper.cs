using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHelper : MonoBehaviour, ISelectHandler, IPointerClickHandler, IPointerEnterHandler
    {
// TODO: add font override to this (like TextHelper)

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _hoverEffectTrigger;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _clickEffectTrigger;

        private Button _button;

#region Unity Lifecycle
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
#endregion

#region Event Handlers
        public void OnSelect(BaseEventData eventData)
        {
            if(null != _hoverEffectTrigger) {
                _hoverEffectTrigger.Trigger();
            } else if(null != UIManager.Instance.DefaultButtonHoverEffectTrigger) {
                UIManager.Instance.DefaultButtonHoverEffectTrigger.Trigger();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _button.Select();
            _button.Highlight();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if(null != _clickEffectTrigger) {
                _clickEffectTrigger.Trigger();
            } else if(null != UIManager.Instance.DefaultButtonClickEffectTrigger) {
                UIManager.Instance.DefaultButtonClickEffectTrigger.Trigger();
            }
        }
#endregion
    }
}
