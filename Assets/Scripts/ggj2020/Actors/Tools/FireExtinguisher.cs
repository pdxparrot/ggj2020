using pdxpartyparrot.Core.Time;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors.Tools
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class FireExtinguisher : Tool
    {
        private ITimer _holdTimer;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0.0f;

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

            HoldingPlayer.UIBubble.SetPressedSprite();
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
