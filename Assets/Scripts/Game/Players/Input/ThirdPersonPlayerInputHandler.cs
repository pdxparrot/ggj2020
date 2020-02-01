using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class ThirdPersonPlayerInputHandler : PlayerInputSystemHandler
    {
        protected override void DoMove(InputAction action)
        {
            Vector2 axes = action.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            OnMove(new Vector3(axes.x, 0.0f, axes.y));
        }
    }
}
