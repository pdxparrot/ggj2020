using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Input;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.UI
{
    public sealed class TitleScreen : MonoBehaviour
    {
        [SerializeField]
        [CanBeNull]
        private EffectTrigger _loadEffectTrigger;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _subTitleText;

        // flag to tell us if we've already started the load effect
        // but also to tell us that the load effect should stop immediately
        // if it's started after this point
        private bool _started;

#region Unity Lifecycle
        private void Awake()
        {
            // TODO: this should be up to the load effect to setup
            /*if(null != _titleText) {
                Color color = _titleText.color;
                color.a = 0.0f;
                _titleText.color = color;
            }

            if(null != _subTitleText) {
                Color color = _subTitleText.color;
                color.a = 0.0f;
                _subTitleText.color = color;
            }*/
        }

        private void Start()
        {
            if(null != _loadEffectTrigger) {
                _loadEffectTrigger.Trigger();
                if(_started) {
                    _loadEffectTrigger.StopTrigger();
                }
            }

            _started = true;
        }

        private void OnEnable()
        {
            InputManager.Instance.EventSystem.UIModule.submit.action.performed += OnSubmit;
            InputManager.Instance.EventSystem.UIModule.cancel.action.performed += OnCancel;
        }

        private void OnDisable()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.cancel.action.performed -= OnCancel;
                InputManager.Instance.EventSystem.UIModule.submit.action.performed -= OnSubmit;
            }
        }
#endregion

        public void FinishLoading()
        {
            if(null != _loadEffectTrigger) {
                if(_loadEffectTrigger.IsRunning) {
                    _loadEffectTrigger.StopTrigger();
                } else if(!_started) {
                    _started = true;
                }
            }
        }

#region Event Handlers
        private void OnSubmit(InputAction.CallbackContext context)
        {
            FinishLoading();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            FinishLoading();
        }
#endregion
    }
}
