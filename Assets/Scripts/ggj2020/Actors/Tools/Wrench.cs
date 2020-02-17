using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class Wrench : Tool
    {
        [Space(10)]

        [SerializeField]
        private EffectTrigger _turnEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _turnRumbleEffectTriggerComponent;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private int _lastTurnAxis = 1;

        [SerializeField]
        [ReadOnly]
        private int _successfulTurns;

        protected override void SetHoldingPlayer([CanBeNull] MechanicBehavior player)
        {
            base.SetHoldingPlayer(player);

            if(null != HoldingPlayer) {
                _turnRumbleEffectTriggerComponent.PlayerInput = HoldingPlayer.Owner.GamePlayerInput.InputHelper;
            } else {
                _turnRumbleEffectTriggerComponent.PlayerInput = null;
            }
        }

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
                HoldingPlayer.ToolBubble.ShowThumbLeft();
            } else {
                HoldingPlayer.ToolBubble.ShowThumbRight();
            }
        }

        public override bool Use()
        {
            if(!base.Use()) {
                return false;
            }

            _lastTurnAxis = 1;
            HoldingPlayer.ToolBubble.ShowThumbLeft();

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
                HoldingPlayer.ToolBubble.ShowThumbLeft();

                _turnEffectTrigger.Trigger();
            } else if ((axis.x <= -0.5f || axis.y <= -0.5f) && _lastTurnAxis != -1) {
                _lastTurnAxis = -1;
                HoldingPlayer.ToolBubble.ShowThumbRight();

                _turnEffectTrigger.Trigger();

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
