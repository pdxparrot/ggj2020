using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class Wrench : Tool
    {
        [SerializeField]
        private int _maxSuccesfulTurns = 5;

        [SerializeField]
        [ReadOnly]
        private int _lastTurnAxis = 1;

        [SerializeField]
        [ReadOnly]
        private int _successfulTurns;

        public override void CanUse()
        {
            if(_lastTurnAxis == 1) {
                HoldingPlayer.Owner.UIBubble.SetThumbLeft();
            } else {
                HoldingPlayer.Owner.UIBubble.SetThumbRight();
            }
        }

        public override bool Use()
        {
            // find a point to repair
            RepairPoint repairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
            if(repairPoint == null) {
                return false;
            }

            if(!SetRepairPoint(repairPoint)) {
                return false;
            }

            if(!base.Use()) {
                return false;
            }

            _lastTurnAxis = 1;
            HoldingPlayer.Owner.UIBubble.SetThumbLeft();

            return true;
        }

        public override void EndUse()
        {
            _successfulTurns = 0;

            base.EndUse();
        }

        public override void TrackThumbStickAxis(Vector2 axis)
        {
            if((axis.x >= 0.5f || axis.y >= 0.5f) && _lastTurnAxis != 1) {
                _lastTurnAxis = 1;
                HoldingPlayer.Owner.UIBubble.SetThumbLeft();
            } else if ((axis.x <= -0.5f || axis.y <= -0.5f) && _lastTurnAxis != -1) {
                _lastTurnAxis = -1;
                HoldingPlayer.Owner.UIBubble.SetThumbRight();

                _successfulTurns++;
            }

            if(_successfulTurns >= _maxSuccesfulTurns) {
                if(!RepairPoint.IsRepaired) {
                    RepairPoint.Repair();
                }
            }
        }

#region Attachments
        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_Wrench", "Tool_Wrench");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_Wrench");
        }
#endregion
    }
}
