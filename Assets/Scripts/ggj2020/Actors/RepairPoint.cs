using System;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    public class RepairPoint : MonoBehaviour
    {
#region Events
        public event EventHandler<EventArgs> RepairedEvent;
#endregion

        [SerializeField]
        private EffectTrigger _repairEffectTrigger;

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

        public bool IsRepaired => RepairState.Repaired == CurrentRepairState;

        public void Repair()
        {
            _repairState = RepairState.Repaired;

            _repairEffectTrigger.Trigger();

            RepairedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
