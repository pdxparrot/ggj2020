using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Actors;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class Hammer : Tool
    {
        // TODO: move to data
        [SerializeField]
        private int _maxSuccesfulHits = 5;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        public override void CanUse()
        {
            if(InUse) {
                HoldingPlayer.Owner.UIBubble.SetPressedSprite();
            } else {
                HoldingPlayer.Owner.UIBubble.SetUnpressedSprite();
            }
        }

        public override bool Use()
        {
            if(UseEffect.IsRunning) {
                return false;
            }

            // find a point to repair if we don't already have one
            RepairPoint repairPoint = RepairPoint;
            if(null == repairPoint) {
                repairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
                if(repairPoint == null) {
                    return false;
                }

                if(!SetRepairPoint(repairPoint)) {
                    return false;
                }
            }

            if(!base.Use()) {
                return false;
            }

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();

            return true;
        }

        public override void EndUse()
        {
            // TODO: if we move away from the repair point
            // then reset the successes to 0

            base.EndUse();
        }

        protected override void OnUseToolEffectEnd()
        {
            base.OnUseToolEffectEnd();

            HoldingPlayer.Owner.UIBubble.SetUnpressedSprite();

            _succesfulHits++;
            if(_succesfulHits < _maxSuccesfulHits) {
                return;
            }

            if(!RepairPoint.IsRepaired) {
                RepairPoint.Repair();
            }

            _succesfulHits = 0;
        }

#region Attachments
        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_Hammer", "Tool_Hammer");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_Hammer");
        }
#endregion
    }
}
