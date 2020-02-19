using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class Hammer : Tool
    {
        [Space(10)]

        [SerializeField]
        private DelayEffectTriggerComponent _delayEffectTriggerComponent;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _delayEffectTriggerComponent.Seconds = GameManager.Instance.GameGameData.HammerCooldown;
        }
#endregion

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
                HoldingPlayer.ToolBubble.ShowPressedButton();
            } else {
                HoldingPlayer.ToolBubble.ShowUnpressedButton();
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

            HoldingPlayer.ToolBubble.ShowPressedButton();

            return true;
        }

        public override void EndUse()
        {
            // ignore everything the base tool does for this
        }

        protected override void OnUseToolEffectEnd()
        {
            base.OnUseToolEffectEnd();

            base.EndUse();

            HoldingPlayer.ToolBubble.ShowUnpressedButton();

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
