using JetBrains.Annotations;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class PlayerInputSystem<T> : PlayerInput where T: class, IInputActionCollection, new()
    {
        [SerializeField]
        [ReadOnly]
        private bool _pollMove;

        protected bool PollMove
        {
            get => _pollMove;
            set => _pollMove = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _pollLook;

        protected bool PollLook
        {
            get => _pollLook;
            set => _pollLook = value;
        }

        [CanBeNull]
        protected abstract InputAction MoveAction { get; }

        [CanBeNull]
        protected abstract InputAction LookAction { get; }

        private PlayerInputHelper _inputHelper;

#region Unity Lifecycle
        protected override void Update()
        {
            base.Update();

            if(PollMove) {
                DoPollMove();
            }

            if(PollLook) {
                DoPollLook();
            }
        }
#endregion

        protected virtual bool IsInputAllowed(InputAction.CallbackContext ctx)
        {
            // no input unless we have focus
            if(!Application.isFocused) {
                return false;
            }

            // ignore keyboard/mouse while the debug menu is open
            if(DebugMenuManager.Instance.Enabled && (ctx.control.device == Keyboard.current || ctx.control.device == Mouse.current)) {
                return false;
            }

            return true;
        }

        protected virtual void DoPollMove()
        {
            if(null == MoveAction) {
                return;
            }

            Vector2 axes = MoveAction.ReadValue<Vector2>();
            OnMove(new Vector3(axes.x, axes.y, 0.0f));
        }

        protected virtual void DoPollLook()
        {
            if(null == LookAction) {
                return;
            }

            Vector2 axes = LookAction.ReadValue<Vector2>();
            OnLook(new Vector3(axes.x, axes.y, 0.0f));
        }

#region Common Actions
        public void OnPause(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Pause: {context.action.phase}");
            }

            if(context.performed) {
                OnPause();
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(context.performed) {
                PollMove = true;
                DoPollMove();
            } else if(context.canceled) {
                PollMove = false;
                OnMove(Vector3.zero);
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(context.performed) {
                PollLook = true;
                DoPollLook();
            } else if(context.canceled) {
                PollLook = false;
                OnMove(Vector3.zero);
            }
        }
#endregion
    }
}
