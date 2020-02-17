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
        private enum WrenchDirection
        {
            Left,
            Right
        }

        [Space(10)]

        [SerializeField]
        private EffectTrigger _turnEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _turnRumbleEffectTriggerComponent;

        [Space(10)]

        [SerializeField]
        [Range(0.1f, 0.9f)]
        private float _turnAxisAmount = 0.9f;

        [SerializeField]
        [ReadOnly]
        private WrenchDirection _lastTurnDirection = WrenchDirection.Right;

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

            if(_lastTurnDirection == WrenchDirection.Right) {
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

            _lastTurnDirection = WrenchDirection.Right;
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
            if(axis.x >= _turnAxisAmount && _lastTurnDirection == WrenchDirection.Left) {
                _lastTurnDirection = WrenchDirection.Right;
                HoldingPlayer.ToolBubble.ShowThumbLeft();

                _turnEffectTrigger.Trigger();
            } else if(axis.x <= -_turnAxisAmount && _lastTurnDirection == WrenchDirection.Right) {
                _lastTurnDirection = WrenchDirection.Left;
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
