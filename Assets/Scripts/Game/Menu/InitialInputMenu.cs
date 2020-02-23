using System.Text;

using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class InitialInputMenu : MenuPanel
    {
        [SerializeField]
        private string _characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvxyz0123456789";

        [SerializeField]
        private InitialInput[] _initials;

        [SerializeField]
        [ReadOnly]
        private int _currentInitialIdx;

        [SerializeField]
        private Button _doneButton;

        [SerializeField]
        [ReadOnly]
        private bool _pollAdvanceLetter;

        [SerializeField]
        private float _pollCooldown = 0.25f;

        [SerializeReference]
        [ReadOnly]
        private ITimer _pollCooldownTimer;

        [SerializeField]
        [ReadOnly]
        private bool _enableButton;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(_initials.Length > 0);

            // don't auto-select anything, we'll control it programatically
            InitialSelection = null;

            base.Awake();

            char[] characters = _characterSet.ToCharArray();
            foreach(InitialInput initial in _initials) {
                initial.Characters = characters;
            }

            _currentInitialIdx = 0;

            _pollCooldownTimer = TimeManager.Instance.AddTimer();

            EnableDoneButton(false);
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_pollCooldownTimer);
                _pollCooldownTimer = null;
            }

            base.OnDestroy();
        }

        protected override void Update()
        {
            base.Update();

            if(_pollAdvanceLetter) {
                AdvanceLetter();
            }
        }

        private void LateUpdate()
        {
            // have to do this in LateUpdate()
            // so we don't end up triggering the button
            // as soon as it's selected
            if(_enableButton) {
                EnableDoneButton(true);
                _enableButton = false;
            }
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            _currentInitialIdx = 0;
            _initials[_currentInitialIdx].Select(true);
        }

        public string GetInitials()
        {
            StringBuilder builder = new StringBuilder();
            foreach(InitialInput initial in _initials) {
                builder.Append(initial.CurrentCharacter);
            }
            return builder.ToString();
        }

        private void NextInitial()
        {
            if(_currentInitialIdx >= 0) {
                _initials[_currentInitialIdx].Select(false);
            }

            _currentInitialIdx++;
            if(_currentInitialIdx >= _initials.Length) {
                _currentInitialIdx = -1;

                _enableButton = true;
            } else {
                _initials[_currentInitialIdx].Select(true);
            }
        }

        private void PreviousInitial()
        {
            if(_currentInitialIdx < 0) {
                _currentInitialIdx = _initials.Length - 1;
                _initials[_currentInitialIdx].Select(true);

                EnableDoneButton(false);
            } else {
                _initials[_currentInitialIdx].Select(false);

                _currentInitialIdx = Mathf.Max(_currentInitialIdx - 1, 0);
                _initials[_currentInitialIdx].Select(true);
            }
        }

        private void EnableDoneButton(bool enable)
        {
            if(enable) {
                _doneButton.Select();
                _doneButton.Highlight();
            }
            _doneButton.interactable = enable;
        }

        private void AdvanceLetter()
        {
            if(_currentInitialIdx < 0 || _pollCooldownTimer.IsRunning) {
                return;
            }

            Vector2 direction = InputManager.Instance.EventSystem.UIModule.move.action.ReadValue<Vector2>();
            if(direction.y > 0.0f) {
                _initials[_currentInitialIdx].NextLetter();
            } else if(direction.y < 0.0f) {
                _initials[_currentInitialIdx].PreviousLetter();
            }

            _pollCooldownTimer.Start(_pollCooldown);
        }

#region Events
        public override void OnSubmit(InputAction.CallbackContext context)
        {
            if(!context.performed || _currentInitialIdx < 0) {
                return;
            }

            NextInitial();
        }

        public override void OnCancel(InputAction.CallbackContext context)
        {
            if(!context.performed) {
                return;
            }

            PreviousInitial();
        }

        public override void OnMove(InputAction.CallbackContext context)
        {
            if(!context.performed || _currentInitialIdx < 0) {
                return;
            }

            if(context.performed) {
                Vector3 direction = context.ReadValue<Vector2>();
                // TODO: this is a little awkward because it's easy
                // to accidentally hit it when going up / down
                /*if(direction.x > 0.0f) {
                    NextInitial();
                } else if(direction.x < 0.0f) {
                    PreviousInitial();
                } else*/ if(direction.y != 0.0f) {
                    _pollAdvanceLetter = true;
                    _pollCooldownTimer.Stop();

                    AdvanceLetter();
                    _pollCooldownTimer.AddTime(_pollCooldown);
                }
            } else if(context.canceled) {
                _pollAdvanceLetter = false;
            }
        }
#endregion
    }
}
