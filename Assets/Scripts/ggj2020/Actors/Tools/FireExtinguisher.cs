using System.Collections;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class FireExtinguisher : Tool
    {
        [Space(10)]

        [SerializeField]
        private EffectTrigger _loopingRumbleEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _loopingRumbleEffectTriggerComponent;

        private ITimer _holdTimer;

        private Coroutine _holdRoutine;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _holdTimer = TimeManager.Instance.AddTimer();
            _holdTimer.TimesUpEvent += (sender, args) => {
                if(!RepairPoint.IsRepaired) {
                    RepairPoint.Repair();
                }
            };
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_holdTimer);
            }

            base.OnDestroy();
        }
#endregion

        protected override void SetHoldingPlayer([CanBeNull] MechanicBehavior player)
        {
            base.SetHoldingPlayer(player);

            if(null != HoldingPlayer) {
                _loopingRumbleEffectTriggerComponent.PlayerInput = HoldingPlayer.Owner.GamePlayerInput.InputHelper;
            } else {
                _loopingRumbleEffectTriggerComponent.PlayerInput = null;
            }
        }

        public override bool SetRepairPoint(RepairPoint repairPoint)
        {
            if(!base.SetRepairPoint(repairPoint)) {
                return false;
            }

            _holdTimer.Stop();

            return true;
        }

        public override void ShowBubble()
        {
            if(!IsHeld) {
                return;
            }

            HoldingPlayer.UIBubble.SetPressedSprite();
        }

        public override void Drop()
        {
            if(null != _holdRoutine) {
                StopCoroutine(_holdRoutine);
            }
            _holdRoutine = null;

            base.Drop();
        }

        public override bool Use()
        {
            if(!base.Use()) {
                return false;
            }

            _holdTimer.Start(GameManager.Instance.GameGameData.FireExtinguisherHoldTime);

            _holdRoutine = StartCoroutine(HoldRoutine());

            return true;
        }

        public override void EndUse()
        {
            _holdTimer.Stop();

            if(null != _holdRoutine) {
                StopCoroutine(_holdRoutine);
            }
            _holdRoutine = null;

            base.EndUse();
        }

        private IEnumerator HoldRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            while(true) {
                yield return wait;

                _loopingRumbleEffectTrigger.Trigger();
            }
        }
    }
}
