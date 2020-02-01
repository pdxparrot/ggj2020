using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class ThirdPersonPlayerInputHandler : PlayerInputSystemHandler
    {
        protected override void DoMove(InputAction.CallbackContext context)
        {
            Vector2 axes = context.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            OnMove(new Vector3(axes.x, 0.0f, axes.y));
        }
    }
}
