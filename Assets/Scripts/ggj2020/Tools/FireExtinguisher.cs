using System;

using pdxpartyparrot.Core.Time;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class FireExtinguisher : Tool
    {
        // TODO: move to data
        [SerializeField]
        private float _holdTime = 1;

        private ITimer _holdTimer;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _holdTimer = TimeManager.Instance.AddTimer();
            _holdTimer.TimesUpEvent += HoldTimerTimesUpEventHandler;
        }

        private void OnDestroy()
        {
            _holdTimer.TimesUpEvent -= HoldTimerTimesUpEventHandler;

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_holdTimer);
            }
        }
#endregion

        public override bool UseTool()
        {
            if(!base.UseTool()) {
                return false;
            }

            // find a point to repair
            RepairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
            if(RepairPoint == null) {
                base.EndUseTool();
                return false;
            }

            _holdTimer.Start(_holdTime);

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();

            HoldingPlayer.FireExtinguisherEffect.gameObject.SetActive(true);
            HoldingPlayer.FireExtinguisherEffect.Trigger();

            return true;
        }

        public override void EndUseTool()
        {
            _holdTimer.Stop();

            HoldingPlayer.FireExtinguisherEffect.StopTrigger();
            HoldingPlayer.FireExtinguisherEffect.gameObject.SetActive(false);

            base.EndUseTool();
        }

        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_FireExtinguisher", "Tool_FireExtinguisher");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_FireExtinguisher");
        }

#region Events
        private void HoldTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            if(!RepairPoint.IsRepaired) {
                RepairPoint.Repair();
            }
            RepairPoint = null;
        }
#endregion
    }
}
