using System;

using pdxpartyparrot.Core.Time;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
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

        protected override void OnDestroy()
        {
            _holdTimer.TimesUpEvent -= HoldTimerTimesUpEventHandler;

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_holdTimer);
            }

            base.OnDestroy();
        }
#endregion

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

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();
        }

        public override bool Use()
        {
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
