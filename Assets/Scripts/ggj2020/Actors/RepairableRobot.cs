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
        public override bool IsLocalActor => true;

        [Space(10)]

        [Header("Repair Points")]

        [SerializeField]
        private RepairPoint _headRepairPoint;

        [SerializeField]
        private RepairPoint _leftArmRepairPoint;

        [SerializeField]
        private RepairPoint _rightArmRepairPoint;

        [SerializeField]
        private RepairPoint _leftLegRepairPoint;

        [SerializeField]
        private RepairPoint _rightLegRepairPoint;

        // TODO: this should be split into a factor per-player
        [SerializeField]
        [ReadOnly]
        private float _chargeLevel;

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

        public void EnterRepairBay(Action onComplete)
        {
            // TODO: figure out what the current repair states are and set them up

            _enterMoveTween.From = transform.position;
            _enterRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }

        public void ReadyForRepair()
        {
            _enterRepairBayEffectTrigger.StopTrigger();
        }

        public void ExitRepairBay(Action onComplete)
        {
            _exitMoveTween.From = Vector3.zero;
            _exitRepairBayEffectTrigger.Trigger(() => {
                onComplete?.Invoke();
            });
        }
    }
}
