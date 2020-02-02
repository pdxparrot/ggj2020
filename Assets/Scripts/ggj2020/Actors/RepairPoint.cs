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
        private EffectTrigger _fireDamageEffectTrigger;

        [SerializeField]
        private EffectTrigger _damagedEffectTrigger;

        [SerializeField]
        private EffectTrigger _looseEffectTrigger;

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

        public void ResetDamage()
        {
            _repairState = RepairState.Repaired;

            StopDamageEffects();

            _repairEffectTrigger.StopTrigger();
        }

        public void Damage()
        {
            _repairState = RepairState.UnRepaired;

            int RepairType = UnityEngine.Random.Range(0, 3);
            switch (RepairType)
            {
                case 0:
                    _damageType = DamageType.Fire;
                    _fireDamageEffectTrigger.Trigger();
                    break;
                case 1:
                    _damageType = DamageType.Damaged;
                    _damagedEffectTrigger.Trigger();
                    break;
                case 2:
                    _damageType = DamageType.Loose;
                    _looseEffectTrigger.Trigger();
                    break;
            }
        }

        public DamageType GetDamageType()
        {
            return RepairPointDamageType;
        }

        public void Repair()
        {
            _repairState = RepairState.Repaired;

            StopDamageEffects();

            _repairEffectTrigger.Trigger();

            RepairedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void StopDamageEffects()
        {
            _fireDamageEffectTrigger.StopTrigger();
            _damagedEffectTrigger.StopTrigger();
            _looseEffectTrigger.StopTrigger();
        }
    }
}
