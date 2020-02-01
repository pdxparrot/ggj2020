﻿using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
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
        private EffectTrigger _exitRepairBayEffectTrigger;

        public void EnterRepairBay()
        {
            // TODO: figure out what the current repair states are and set them up

            _enterRepairBayEffectTrigger.Trigger();
        }

        public void ReadyForRepair()
        {
            _enterRepairBayEffectTrigger.StopTrigger();
        }

        public void ExitRepairBay()
        {
            _exitRepairBayEffectTrigger.Trigger();
        }
    }
}