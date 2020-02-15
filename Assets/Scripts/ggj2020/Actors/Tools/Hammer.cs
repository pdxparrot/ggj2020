using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class Hammer : Tool
    {
        // TODO: move to data
        [SerializeField]
        private int _maxSuccesfulHits = 5;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        public override bool SetRepairPoint(RepairPoint repairPoint)
        {
            if(!base.SetRepairPoint(repairPoint)) {
                return false;
            }

            _succesfulHits = 0;

            return true;
        }

        public override void ShowBubble()
        {
            if(!IsHeld) {
                return;
            }

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

            if(!base.Use()) {
                return false;
            }

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();

            return true;
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
