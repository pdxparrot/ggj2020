using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Tween;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public class RepairableRobot : Actor3D
    {
#region Events
        public event EventHandler<EventArgs> RepairedEvent;
#endregion

        public override bool IsLocalActor => true;

        [Space(10)]

        [SerializeField]
        private RepairPoint[] _repairPoints;

        // TODO: this should be split into a factor per-player
        [SerializeField]
        [ReadOnly]
        private float _chargeLevel;

        public float ChargeLevel => _chargeLevel;

        [Space(10)]

        [Header("Effects")]

        [SerializeField]
        private EffectTrigger _enterRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _enterMoveTween;

        [SerializeField]
        private EffectTrigger _exitRepairBayEffectTrigger;

        [SerializeField]
        private TweenMove _exitMoveTween;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.RepairedEvent += RepairedEventHandler;
            }
        }

        protected override void OnDestroy()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                repairPoint.RepairedEvent -= RepairedEventHandler;
            }

            base.OnDestroy();
        }
#endregion

        public bool IsRepared()
        {
            foreach(RepairPoint repairPoint in _repairPoints) {
                if(!repairPoint.IsRepaired) {
                    return false;
                }
            }
            return true;
        }

        public void EnterRepairBay(Action onComplete)
        {
            // TODO: figure out what the current repair states are and set them up

            _enterMoveTween.From = transform.position;
            _enterMoveTween.To = Vector3.zero;

            _enterRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }

        public void ExitRepairBay(Action onComplete)
        {
            _exitMoveTween.From = Vector3.zero;
            _exitMoveTween.To = GameManager.Instance.GameLevelHelper.RepairableExit.position;

            _exitRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }

#region Events
        private void RepairedEventHandler(object sender, EventArgs args)
        {
            if(IsRepared()) {
                RepairedEvent?.Invoke(this, EventArgs.Empty);
            }
        }
#endregion
    }
}
