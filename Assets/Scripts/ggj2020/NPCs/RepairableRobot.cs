using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.NPCs
{
    public class RepairableRobot : NPC25D
    {
        [Space(10)]

        [Header("Repair Points")]

        private RepairPoint _headRepairPoint;

        private RepairPoint _leftArmRepairPoint;

        private RepairPoint _rightArmRepairPoint;

        private RepairPoint _leftLegRepairPoint;

        private RepairPoint _rightLegRepairPoint;

        // TODO: this should be split into a factor per-player
        [SerializeField]
        [ReadOnly]
        private float _chargeLevel;

        [Space(10)]

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
