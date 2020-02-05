using System;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    [RequireComponent(typeof(AudioSource))]
    public class RepairPoint : MonoBehaviour, IInteractable
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
            }
        }

        public void Repair()
        {
            _repairState = RepairState.Repaired;

            gameObject.SetActive(false);
            StopDamageEffects();

            _repairEffectTrigger.Trigger();

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
