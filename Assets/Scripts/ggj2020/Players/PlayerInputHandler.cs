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

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInputData is PlayerInputData);
            Assert.IsTrue(Player is Player);
        }
#endregion

#region Actions
        public void OnInteractAction(InputAction.CallbackContext context)
        {
            if(!IsInputAllowed(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Interact: {context.action.phase}");
            }

            if(context.performed) {
                Debug.LogWarning("TODO: context interact (ladder, tool, etc)");
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
                Debug.LogWarning("TODO: context cancel (drop tool, etc)");
            }
        }
#endregion
    }
}
