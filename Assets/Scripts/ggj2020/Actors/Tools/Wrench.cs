using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class Wrench : Tool
    {
        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private int _lastTurnAxis = 1;

        [SerializeField]
        [ReadOnly]
        private int _successfulTurns;

        public override bool SetRepairPoint(RepairPoint repairPoint)
        {
            if(!base.SetRepairPoint(repairPoint)) {
                return false;
            }

            _successfulTurns = 0;

            return true;
        }

        public override void ShowBubble()
        {
            if(!IsHeld) {
                return;
            }

            if(_lastTurnAxis == 1) {
                HoldingPlayer.UIBubble.SetThumbLeft();
            } else {
                HoldingPlayer.UIBubble.SetThumbRight();
            }
        }

        public override bool Use()
        {
            if(!base.Use()) {
                return false;
            }

            _lastTurnAxis = 1;
            HoldingPlayer.UIBubble.SetThumbLeft();

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
                HoldingPlayer.UIBubble.SetThumbLeft();
            } else if ((axis.x <= -0.5f || axis.y <= -0.5f) && _lastTurnAxis != -1) {
                _lastTurnAxis = -1;
                HoldingPlayer.UIBubble.SetThumbRight();

                _successfulTurns++;
            }

            if(_successfulTurns >= GameManager.Instance.GameGameData.WrenchSuccessfulTurns) {
                if(!RepairPoint.IsRepaired) {
                    RepairPoint.Repair();
                }
            }
        }
    }
}
