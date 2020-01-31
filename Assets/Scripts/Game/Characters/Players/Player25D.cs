using pdxpartyparrot.Core.Math;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class Player25D : Player3D
    {
        public override void SetFacing(Vector3 direction)
        {
            direction = new Vector3(direction.x, 0.0f, 0.0f);

            if(direction.sqrMagnitude < MathUtil.Epsilon) {
                return;
            }

            base.SetFacing(direction);
        }
    }
}
