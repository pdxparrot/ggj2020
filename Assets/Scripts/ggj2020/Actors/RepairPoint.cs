using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public class RepairPoint : MonoBehaviour
    {
        public enum DamageType
        {
            Fire,
            Damaged,
            Loose,
        }

        public enum RepairState
        {
            UnRepaired,
            Repaired,
        }

        [SerializeField]
        private DamageType _damageType = DamageType.Fire;

        public DamageType RepairPointDamageType => _damageType;

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
