using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.ggj2020.Actors;

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

        public override void CanUse()
        {
            HoldingPlayer.Owner.UIBubble.SetPressedSprite();
        }

        public override bool Use()
        {
            // find a point to repair
            RepairPoint repairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
            if(repairPoint == null) {
                return false;
            }

            if(!SetRepairPoint(repairPoint)) {
                return false;
            }

            if(!base.Use()) {
                return false;
            }

            _holdTimer.Start(_holdTime);

            return true;
        }

        public override void EndUse()
        {
            _holdTimer.Stop();

            base.EndUse();
        }

#region Attachments
        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_FireExtinguisher", "Tool_FireExtinguisher");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_FireExtinguisher");
        }
#endregion

#region Events
        private void HoldTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            if(!RepairPoint.IsRepaired) {
                RepairPoint.Repair();
            }
        }
#endregion
    }
}
