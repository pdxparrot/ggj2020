using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.NPCs
{
    public class RepairPoint : MonoBehaviour
    {
        public enum RepairState
        {
            Repaired,

            OnFire,
            Damaged,
            Loose,
        }

        [SerializeField]
        [ReadOnly]
        private RepairState _repairState = RepairState.Repaired;

        public RepairState CurrentRepairState => _repairState;

        public void SetRepairState(RepairState repairState)
        {
            _repairState = repairState;

            // TODO: this should kick off some effect
        }
    }
}
