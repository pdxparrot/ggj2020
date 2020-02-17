using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
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
        public bool CanInteract => true;

        public Type InteractableType => GetType();

#region Art
        [SerializeField]
        private GameObject _chargingStationOn;

        [SerializeField]
        private GameObject _chargingStationOff;

        [SerializeField]
        private GameObject _chargingStationUI;
#endregion

        [Space(10)]

#region Effects
        [SerializeField]
        private RumbleEffectTriggerComponent _useRumbleEffectTriggerComponent;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private MechanicBehavior _usingPlayer;

        public bool IsInUse => _usingPlayer != null;

        [Space(10)]

        private readonly HashSet<MechanicBehavior> _succesfulPlayers = new HashSet<MechanicBehavior>();

#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
            GetComponent<AudioSource>().spatialBlend = 0.0f;
        }
#endregion

        public void Enable(bool enable)
        {
            if(enable) {
                _chargingStationOn.SetActive(true);
                _chargingStationOff.SetActive(false);
            } else {
                _chargingStationOn.SetActive(false);
                _chargingStationOff.SetActive(true);
            }
        }

        public void EnableUI(bool enable)
        {
            _chargingStationUI.SetActive(enable);
        }

        public void Reset()
        {
            _succesfulPlayers.Clear();
        }

        private void SetUsingPlayer(MechanicBehavior player)
        {
            _usingPlayer = player;

            if(null != _usingPlayer) {
                _useRumbleEffectTriggerComponent.PlayerInput = _usingPlayer.Owner.GamePlayerInput.InputHelper;
            } else {
                _useRumbleEffectTriggerComponent.PlayerInput = null;
            }
        }

        public bool Use(MechanicBehavior player)
        {
            if(!GameManager.Instance.MechanicsCanInteract || IsInUse || _succesfulPlayers.Contains(player)) {
                return false;
            }

            SetUsingPlayer(player);

            // TODO: other things

            return true;
        }
    }
}
