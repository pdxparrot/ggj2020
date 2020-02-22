using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    // TODO: make this an actor
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public sealed class ChargingStation : MonoBehaviour, IInteractable
    {
#region Events
        public event EventHandler<EventArgs> ChargeCompleteEvent;
#endregion

        private enum EnabledUI
        {
            Robot,
            Battery,
            PlayerInteract,
        }

        public bool CanInteract => true;

        public Type InteractableType => GetType();

#region Art
        [SerializeField]
        private float _animateTime = 1.0f;

        [SerializeField]
        private GameObject _chargingStationOn;

        [SerializeField]
        private GameObject _chargingStationOff;

        [SerializeField]
        private GameObject _robotIcon;

        [SerializeField]
        private GameObject _batteryIcon;

        [SerializeField]
        private GameObject _playerInteractUI;

        [SerializeField]
        [ReadOnly]
        private EnabledUI _enabledUI = EnabledUI.Robot;

        [SerializeField]
        private GameObject _chargingStationUI;
#endregion

        [Space(10)]

#region Effects
        [SerializeField]
        private EffectTrigger _useEffect;

        [SerializeField]
        private EffectTrigger _loopingRumbleEffectTrigger;

        [SerializeField]
        private RumbleEffectTriggerComponent _loopingRumbleEffectTriggerComponent;

        [SerializeField]
        private EffectTrigger _chargeCompleteEffect;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _enabled;

        public bool Enabled => _enabled;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private MechanicBehavior _usingPlayer;

        public bool IsInUse => _usingPlayer != null;

        public bool IsCharged => !_enabled || _succesfulPlayers.Count >= PlayerManager.Instance.PlayerCount;

        private ITimer _holdTimer;

        private Coroutine _holdRoutine;

        private readonly HashSet<MechanicBehavior> _succesfulPlayers = new HashSet<MechanicBehavior>();

#region Unity Lifecycle
        private void Awake()
        {
            // TODO: this should come from an actor data object for this
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

            GetComponent<Collider>().isTrigger = true;
            GetComponent<AudioSource>().spatialBlend = 0.0f;

            _holdTimer = TimeManager.Instance.AddTimer();
            _holdTimer.TimesUpEvent += ChargeTimerTimesUpEventHandler;

            GameManager.Instance.MechanicsCanInteractEvent += MechanicsCanInteractEventHandler;

            StartCoroutine(AnimateRoutine());
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.MechanicsCanInteractEvent -= MechanicsCanInteractEventHandler;
            }

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_holdTimer);
            }
        }
#endregion

        public void Enable(bool enable)
        {
            _enabled = enable;

            if(enable) {
                _chargingStationOn.SetActive(true);
                _chargingStationOff.SetActive(false);

                SetUI(EnabledUI.Robot);
            } else {
                _chargingStationOn.SetActive(false);
                _chargingStationOff.SetActive(true);
            }
        }

        public void EnableUI(bool enable)
        {
            _chargingStationUI.SetActive(enable);
        }

        private void SetUI(EnabledUI enabledUI)
        {
            _enabledUI = enabledUI;
            switch(_enabledUI)
            {
            case EnabledUI.PlayerInteract:
                _robotIcon.SetActive(false);
                _batteryIcon.SetActive(false);
                _playerInteractUI.SetActive(true);
                break;
            case EnabledUI.Robot:
                _robotIcon.SetActive(true);
                _batteryIcon.SetActive(false);
                _playerInteractUI.SetActive(false);
                break;
            case EnabledUI.Battery:
                _robotIcon.SetActive(false);
                _batteryIcon.SetActive(true);
                _playerInteractUI.SetActive(false);
                break;
            }
        }

        public void ResetCharge()
        {
            Debug.Log("Resetting charge");

            _succesfulPlayers.Clear();
        }

        public bool CanUse(MechanicBehavior player)
        {
            return GameManager.Instance.MechanicsCanInteract && Enabled && !IsInUse && !_succesfulPlayers.Contains(player);
        }

        private void SetUsingPlayer(MechanicBehavior player)
        {
            _usingPlayer = player;

            if(null != _usingPlayer) {
                _loopingRumbleEffectTriggerComponent.PlayerInput = _usingPlayer.Owner.GamePlayerInput.InputHelper;
            } else {
                _loopingRumbleEffectTriggerComponent.PlayerInput = null;
            }
        }

        public bool Use(MechanicBehavior player)
        {
            if(!CanUse(player)) {
                return false;
            }

            SetUsingPlayer(player);

            _useEffect.Trigger();

            _holdTimer.Start(GameManager.Instance.GameGameData.ChargeTime);

            _holdRoutine = StartCoroutine(HoldRoutine());

            SetUI(EnabledUI.PlayerInteract);

            return true;
        }

        public void EndUse()
        {
            _holdTimer.Stop();

            if(null != _holdRoutine) {
                StopCoroutine(_holdRoutine);
            }
            _holdRoutine = null;

            _useEffect.KillTrigger();

            _usingPlayer = null;

            SetUI(EnabledUI.Robot);
        }

        private IEnumerator HoldRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.5f);
            while(true) {
                yield return wait;

                if(PartyParrotManager.Instance.IsPaused) {
                    continue;
                }

                _loopingRumbleEffectTrigger.Trigger();
            }
        }

        private IEnumerator AnimateRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_animateTime);
            while(true) {
                yield return wait;

                switch(_enabledUI)
                {
                case EnabledUI.PlayerInteract:
                    break;
                case EnabledUI.Robot:
                    SetUI(EnabledUI.Battery);
                    break;
                case EnabledUI.Battery:
                    SetUI(EnabledUI.Robot);
                    break;
                }
            }
        }

#region Events
        private void MechanicsCanInteractEventHandler(object sender, EventArgs args)
        {
            if(IsInUse && !CanUse(_usingPlayer)) {
                EndUse();
            }
        }

        private void ChargeTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Successful charge!");

            _succesfulPlayers.Add(_usingPlayer);

            _usingPlayer.EndUseChargingStation();

            if(IsCharged) {
                _chargeCompleteEffect.Trigger();

                ChargeCompleteEvent?.Invoke(this, EventArgs.Empty);
            }
        }
#endregion
    }
}
