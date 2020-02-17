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
                HoldingPlayer.ToolBubble.ShowPressedSprite();
            } else {
                HoldingPlayer.ToolBubble.ShowUnpressedSprite();
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

            HoldingPlayer.ToolBubble.ShowPressedSprite();

            return true;
        }

        protected override void OnUseToolEffectEnd()
        {
            base.OnUseToolEffectEnd();

            HoldingPlayer.ToolBubble.ShowUnpressedSprite();

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
