using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class PlayerInputSystemHandler : PlayerInputHandler
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

        [SerializeField]
        [CanBeNull]
        private InputActionReference _moveAction;

        protected InputActionReference MoveAction => _moveAction;

        [SerializeField]
        [CanBeNull]
        private InputActionReference _lookAction;

        protected InputActionReference LookAction => _lookAction;

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

        protected virtual void DoPollMove()
        {
            if(null == _moveAction) {
                return;
            }

            Vector2 axes = _moveAction.action.ReadValue<Vector2>();
            OnMove(new Vector3(axes.x, axes.y, 0.0f));
        }

        protected virtual void DoPollLook()
        {
            if(null == _lookAction) {
                return;
            }

            Vector2 axes = _lookAction.action.ReadValue<Vector2>();
            OnLook(new Vector3(axes.x, axes.y, 0.0f));
        }

#region Common Actions
        public void OnPauseAction(InputAction.CallbackContext context)
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

        public void OnMoveAction(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            /*if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Move: {context.action.phase}");
            }*/

            if(context.performed) {
                PollMove = true;
                DoPollMove();
            } else if(context.canceled) {
                PollMove = false;
                OnMove(Vector3.zero);
            }
        }

        public void OnLookAction(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            /*if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Look: {context.action.phase}");
            }*/

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
