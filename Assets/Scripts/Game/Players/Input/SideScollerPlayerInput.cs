using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class SideScollerPlayerInput<T> : PlayerInputSystem<T> where T: class, IInputActionCollection, new()
    {
        protected override void DoPollMove()
        {
            if(null == MoveAction) {
                return;
            }

            Vector2 axes = MoveAction.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            OnMove(new Vector3(axes.x, 0.0f, axes.y));
        }
    }
}
