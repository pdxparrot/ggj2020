using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Game.Characters.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.NPCs
{
    public class RepairableRobot : NPC25D
    {
        [Space(10)]

        [SerializeField]
        private EffectTrigger _enterRepairBayEffectTrigger;

        [SerializeField]
        private EffectTrigger _exitRepairBayEffectTrigger;

        public void EnterRepairBay()
        {
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
