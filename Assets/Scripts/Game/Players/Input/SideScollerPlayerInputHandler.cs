using UnityEngine;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class SideScollerPlayerInputHandler : PlayerInputSystemHandler
    {
        protected override void DoPollMove()
        {
            if(null == MoveAction) {
                return;
            }

            Vector2 axes = MoveAction.action.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            OnMove(new Vector3(axes.x, 0.0f, axes.y));
        }
    }
}
