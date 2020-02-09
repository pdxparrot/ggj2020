using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class BatteryStation : MonoBehaviour
    {
        // TODO: move to data
        [SerializeField]
        private int _maxSuccesfulHits = 25;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Mechanic _usingPlayer;

        [CanBeNull]
        private Mechanic UsingPlayer
        {
            get => _usingPlayer;
            set => _usingPlayer = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _inUse;

        public bool InUse => _inUse;

        private readonly HashSet<Mechanic> _succesfulPlayers = new HashSet<Mechanic>();

#region Unity Lifecycle
        private void OnTriggerExit(Collider other)
        {
            Player player = other.gameObject.GetComponent<Player>();
            if(null == player) {
                return;
            }

            if(UsingPlayer == player.Mechanic) {
                UsingPlayer = null;
                return;
            }

            _succesfulHits = 0;
        }
#endregion

        public bool Use()
        {
            if(!GameManager.Instance.MechanicsCanInteract || !InUse) {
                return false;
            }

            _inUse = true;

            _succesfulHits++;
            if(_succesfulHits >= _maxSuccesfulHits) {
                _succesfulPlayers.Add(UsingPlayer);

                Debug.LogWarning("Succesful Fix TODO Hook up to Robo");
            }

            return true;
        }
    }
}
