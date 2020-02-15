using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Actors
{
    // TODO: make this an actor
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public sealed class ChargingStation : MonoBehaviour, IInteractable
    {
        public bool CanInteract => true;

        public Type InteractableType => GetType();

        [SerializeField]
        private GameObject _chargingStationOn;

        [SerializeField]
        private GameObject _chargingStationOff;

        // TODO: move to data
        [SerializeField]
        private int _maxSuccesfulHits = 25;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private MechanicBehavior _usingPlayer;

        private readonly HashSet<MechanicBehavior> _succesfulPlayers = new HashSet<MechanicBehavior>();

#region Unity Lifecycle
        private void Awake()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerExit(Collider other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(null == player) {
                return;
            }

            if(_usingPlayer == player.Mechanic) {
                _usingPlayer = null;
                return;
            }

            _succesfulHits = 0;
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

        public bool Use(MechanicBehavior player)
        {
            if(!GameManager.Instance.MechanicsCanInteract || null != _usingPlayer) {
                return false;
            }

            _usingPlayer = player;

            _succesfulHits++;
            if(_succesfulHits >= _maxSuccesfulHits) {
                _succesfulPlayers.Add(_usingPlayer);
                _usingPlayer = null;
            }

            return true;
        }
    }
}
