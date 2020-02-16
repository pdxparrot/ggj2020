using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class Hammer : Tool
    {
        [Space(10)]

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
                HoldingPlayer.UIBubble.SetPressedSprite();
            } else {
                HoldingPlayer.UIBubble.SetUnpressedSprite();
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

            HoldingPlayer.UIBubble.SetPressedSprite();

            return true;
        }

        protected override void OnUseToolEffectEnd()
        {
            base.OnUseToolEffectEnd();

            HoldingPlayer.UIBubble.SetUnpressedSprite();

            _succesfulHits++;
            if(_succesfulHits < GameManager.Instance.GameGameData.HammerSuccessfulHits) {
                return;
            }

            if(!RepairPoint.IsRepaired) {
                RepairPoint.Repair();
            }
        }
    }
}
