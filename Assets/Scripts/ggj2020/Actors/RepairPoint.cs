using System;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;

using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(BoneFollower))]
    public sealed class RepairPoint : MonoBehaviour, IInteractable
    {
#region Events
        public event EventHandler<EventArgs> RepairedEvent;
#endregion

        public enum DamageType
        {
            Fire,
            Damaged,
            Loose,
            Random,
            //Stacked,
        }
        private const int DamageTypeCount = 3;

        public enum RepairState
        {
            Repaired,
            UnRepaired,
        }

        public Type InteractableType => GetType();

#region Effects
        [Header("Effects")]

        [SerializeField]
        private EffectTrigger _fireDamageEffectTrigger;

        [SerializeField]
        private EffectTrigger _damagedEffectTrigger;

        [SerializeField]
        private EffectTrigger _looseEffectTrigger;

        [SerializeField]
        private EffectTrigger _repairEffectTrigger;
#endregion

        [Space(10)]

        [SerializeField]
        private DamageType _damageType = DamageType.Random;

        [SerializeField]
        [ReadOnly]
        private DamageType _currentDamageType = DamageType.Fire;

        public DamageType CurrentDamageType => _currentDamageType;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private RepairState _repairState = RepairState.Repaired;

        public RepairState CurrentRepairState => _repairState;

        public bool IsRepaired => RepairState.Repaired == CurrentRepairState;

        public bool CanInteract => !IsRepaired;

#region Unity Lifecycle
        private void Awake()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0.0f;
        }
#endregion

        public void Initialize()
        {
            ResetDamage();
        }

        public void ResetDamage()
        {
            _repairState = RepairState.Repaired;

            StopDamageEffects();

            _repairEffectTrigger.StopTrigger();
        }

        public void Damage()
        {
            _repairState = RepairState.UnRepaired;

            gameObject.SetActive(true);

            switch(_damageType)
            {
            case DamageType.Fire:
            case DamageType.Damaged:
            case DamageType.Loose:
                _currentDamageType = _damageType;
                break;
            case DamageType.Random:
                _currentDamageType = (DamageType)PartyParrotManager.Instance.Random.Next(DamageTypeCount);
                break;
            /*case DamageType.Stacked:
                break;*/
            }

            InitDamage();
        }

        private void InitDamage()
        {
            switch(_currentDamageType)
            {
            case DamageType.Fire:
                _fireDamageEffectTrigger.Trigger();
                break;
            case DamageType.Damaged:
                _damagedEffectTrigger.Trigger();
                break;
            case DamageType.Loose:
                _looseEffectTrigger.Trigger();
                break;
            default:
                Debug.LogError($"Invalid damage type {_currentDamageType}");
                break;
            }
        }

        public void Repair()
        {
            Debug.Log("Point repair successful!");

            _repairState = RepairState.Repaired;

            StopDamageEffects();

            //_repairEffectTrigger.Trigger();

            RepairedEvent?.Invoke(this, EventArgs.Empty);

            gameObject.SetActive(false);
        }

        private void StopDamageEffects()
        {
            _fireDamageEffectTrigger.StopTrigger();
            _damagedEffectTrigger.StopTrigger();
            _looseEffectTrigger.StopTrigger();
        }
    }
}
