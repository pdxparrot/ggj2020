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
        private MechanicBehavior _usingPlayer;

        private readonly HashSet<MechanicBehavior> _succesfulPlayers = new HashSet<MechanicBehavior>();

#region Unity Lifecycle
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
