using System;

using pdxpartyparrot.Core.Audio;
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
            //Random,
            //Stacked,
        }

        public enum RepairState
        {
            UnRepaired,
            Repaired,
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
        private DamageType _damageType = DamageType.Fire;

        public DamageType RepairPointDamageType => _damageType;

        [SerializeField]
        [ReadOnly]
        private RepairState _repairState = RepairState.Repaired;

        public RepairState CurrentRepairState => _repairState;

        public bool IsRepaired => RepairState.Repaired == CurrentRepairState;

        public bool CanInteract => !IsRepaired;

        private AudioSource _audioSource;

#region Unity Lifecycle
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            AudioManager.Instance.InitSFXAudioMixerGroup(_audioSource);
            _audioSource.loop = true;
            _audioSource.spatialBlend = 0.0f;
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

            // TODO: instead of disabling the entire thing just disable the vfx
            gameObject.SetActive(true);

            switch(RepairPointDamageType)
            {
            case DamageType.Fire:
                _fireDamageEffectTrigger.Trigger();
                _audioSource.clip = GameManager.Instance.GameGameData.RepairableRobotData.FireAudioClip;
                _audioSource.Play();
                break;
            case DamageType.Damaged:
                _damagedEffectTrigger.Trigger();
                _audioSource.clip = GameManager.Instance.GameGameData.RepairableRobotData.DamagedAudioClip;
                _audioSource.Play();
                break;
            case DamageType.Loose:
                _looseEffectTrigger.Trigger();
                _audioSource.clip = GameManager.Instance.GameGameData.RepairableRobotData.LooseAudioClip;
                _audioSource.Play();
                break;
            /*case DamageType.Random:
                break;*/
            /*case DamageType.Stacked:
                break;*/
            }
        }

        public void Repair()
        {
            _repairState = RepairState.Repaired;

            // TODO: instead of disabling the entire thing just disable the vfx
            gameObject.SetActive(false);

            StopDamageEffects();

            //_repairEffectTrigger.Trigger();

            RepairedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void StopDamageEffects()
        {
            if(null != _audioSource) {
                _audioSource.Stop();
            }

            _fireDamageEffectTrigger.StopTrigger();
            _damagedEffectTrigger.StopTrigger();
            _looseEffectTrigger.StopTrigger();
        }
    }
}
