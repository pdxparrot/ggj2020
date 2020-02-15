using pdxpartyparrot.Core.Time;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    public sealed class FireExtinguisher : Tool
    {
        private ITimer _holdTimer;

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

            _holdTimer.Start(GameManager.Instance.GameGameData.FireExtinguisherHoldTime);

            return true;
        }

        public override void EndUse()
        {
            _holdTimer.Stop();

            base.EndUse();
        }
    }
}
