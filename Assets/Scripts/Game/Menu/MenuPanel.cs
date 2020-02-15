using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.UI;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    // TODO: should this be abstract?
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField]
        private Menu _owner;

        protected Menu Owner => _owner;

        [SerializeField]
        [CanBeNull]
        private Button _initialSelection;

        protected Button InitialSelection
        {
            get => _initialSelection;
            set => _initialSelection = value;
        }

#region Effects
        [SerializeField]
        [CanBeNull]
        private EffectTrigger _enableEffect;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _disableEffect;
#endregion

#region Unity Lifecycle
        protected virtual void Awake()
        {
            if(null != _initialSelection) {
                _initialSelection.Select();
                _initialSelection.Highlight();
            }
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            InputManager.Instance.EventSystem.UIModule.submit.action.performed += OnSubmit;
            InputManager.Instance.EventSystem.UIModule.cancel.action.performed += OnCancel;
            InputManager.Instance.EventSystem.UIModule.move.action.performed += OnMove;

            if(null != _enableEffect) {
                _enableEffect.Trigger();
            }
        }

        protected virtual void OnDisable()
        {
            if(null != _disableEffect) {
                _disableEffect.Trigger();
            }

            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.move.action.performed -= OnMove;
                InputManager.Instance.EventSystem.UIModule.cancel.action.performed -= OnCancel;
                InputManager.Instance.EventSystem.UIModule.submit.action.performed -= OnSubmit;
            }
        }

        protected virtual void Update()
        {
            if(null == _initialSelection) {	
                return;	
            }	

            if(null == InputManager.Instance.EventSystem.EventSystem.currentSelectedGameObject ||
                (!InputManager.Instance.EventSystem.EventSystem.currentSelectedGameObject.activeInHierarchy && _initialSelection.gameObject.activeInHierarchy)) {
                _initialSelection.Select();
                _initialSelection.Highlight();
            }
        }
#endregion

        public virtual void Initialize()
        {
        }

        public virtual void ResetMenu()
        {
            if(null == _initialSelection) {
                return;
            }

            _initialSelection.Select();
            _initialSelection.Highlight();
        }

#region Event Handlers
        // this is for buttons
        public virtual void OnBack()
        {
            Owner.PopPanel();
        }

        // these are for input
        // TODO: do these actually need to be public??
        public virtual void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public virtual void OnCancel(InputAction.CallbackContext context)
        {
            OnBack();
        }

        public virtual void OnMove(InputAction.CallbackContext context)
        {
        }
#endregion
    }
}
