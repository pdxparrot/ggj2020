using pdxpartyparrot.Game.Players.Input;
using pdxpartyparrot.ggj2020.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class PlayerInputHandler : SideScollerPlayerInputHandler
    {
        private PlayerInputData GamePlayerInputData => (PlayerInputData)PlayerInputData;

        private Player GamePlayer => (Player)Player;

        private bool ActionActive = false;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInputData is PlayerInputData);
            Assert.IsTrue(Player is Player);
        }
#endregion

        private void DoUseLadder()
        {
            if(GamePlayer.Mechanic.IsOnLadder) {
                GamePlayer.Mechanic.ClimbLadder(false);
            } else if(GamePlayer.Mechanic.CanUseLadder) {
                GamePlayer.Mechanic.ClimbLadder(true);
            }
        }

#region Actions
        protected override void DoMove(InputAction action)
        {

            Vector2 axes = action.ReadValue<Vector2>();

            if (ActionActive)
            {
                GamePlayer.Mechanic.TrackThumbStickAxis(axes);
                return;
            }

            if(GamePlayer.Mechanic.IsOnLadder) {
                OnMove(new Vector3(0.0f, axes.y, 0.0f));
                return;
            }

            base.DoMove(action);
        }

        public void OnUseLadder(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Use ladder: {context.action.phase}");
            }

            if(context.performed) {
                DoUseLadder();
            }
        }

        public void OnInteractAction(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Interact: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.Mechanic.UseOrPickupTool();
                ActionActive = true;
            } else {
                GamePlayer.Mechanic.UseEnded();
                ActionActive = false;
            }
        }

        public void OnCancelAction(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Cancel: {context.action.phase}");
            }

            if(context.performed) {
                GamePlayer.Mechanic.DropTool();
            }
        }
#endregion
    }
}
