using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class PlayerInputSystemHandler : PlayerInputHandler
    {
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

        protected virtual void DoMove(InputAction.CallbackContext context)
        {
            Vector2 axes = context.ReadValue<Vector2>();
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
                DoMove(context);
            } else if(context.canceled) {
                OnMove(Vector3.zero);
            }
        }

        protected virtual void DoLook(InputAction.CallbackContext context)
        {
            Vector2 axes = context.action.ReadValue<Vector2>();
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
                DoLook(context);
            } else if(context.canceled) {
                OnMove(Vector3.zero);
            }
        }
#endregion
    }
}
