using System.Collections.Generic;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ggj2020.Players;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class BatteryStation : Tool
    {
        // TODO: move to data
        [SerializeField]
        private int _maxSuccesfulHits = 25;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        private readonly HashSet<Mechanic> _succesfulPlayers = new HashSet<Mechanic>();

        public override bool SetHeld(Mechanic player)
        {
            if(IsHeld) {
                return false;
            }

            HoldingPlayer = player;

            return true;
        }

        public override void Drop()
        {
            HoldingPlayer = null;
        }

        public override void PlayerExitTrigger()
        {
            if(HoldingPlayer == null) {
                return;
            }

            HoldingPlayer.DropTool();

            _succesfulHits = 0;
        }

        public override bool UseTool()
        {
            if(!base.UseTool()) {
                return false;
            }

            _succesfulHits++;
            if(_succesfulHits >= _maxSuccesfulHits) {
                _succesfulPlayers.Add(HoldingPlayer);

                Debug.LogWarning("Succesful Fix TODO Hook up to Robo");
            }

            return true;
        }
    }
}
