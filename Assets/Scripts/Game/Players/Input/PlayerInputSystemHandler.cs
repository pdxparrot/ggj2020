using pdxpartyparrot.Core.Input;
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

        private InputAction _moveAction;

        private InputAction _lookAction;

#region Unity Lifecycle	
        protected override void Awake()
        {
            base.Awake();

            _moveAction = InputHelper.PlayerInput.actions.FindAction(InputManager.Instance.InputData.MoveActionName);
            _lookAction = InputHelper.PlayerInput.actions.FindAction(InputManager.Instance.InputData.LookActionName);
        }

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
            if(!InputAllowed || null == _moveAction) {	
                return;	
            }

            DoMove(_moveAction);
        }	

        protected virtual void DoPollLook()	
        {	
            if(!InputAllowed || null == _lookAction) {	
                return;	
            }	

            DoLook(_lookAction);	
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

        protected virtual void DoMove(InputAction action)
        {
            Vector2 axes = action.ReadValue<Vector2>();
            OnMove(new Vector3(axes.x, axes.y, 0.0f));
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

        protected virtual void DoLook(InputAction action)
        {
            Vector2 axes = action.ReadValue<Vector2>();
            OnLook(new Vector3(axes.x, axes.y, 0.0f));
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
                OnLook(Vector3.zero);
            }
        }
#endregion
    }
}
