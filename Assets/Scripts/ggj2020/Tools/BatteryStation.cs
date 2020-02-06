using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Players;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class BatteryStation : Tool
    {
        public int MaxSuccesfulHits = 25;
        private int SuccesfulHits = 0;

        List<GameObject> SuccesfulPlayers = new List<GameObject>();

        public override void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
        }

        public override void Drop()
        {

        }

        public override void PlayerExitTrigger()
        {
            if (HoldingPlayer == null)
                return;

            HoldingPlayer.DropTool();
            HoldingPlayer = null;
            SuccesfulHits = 0;
        }

        public override void UseTool(Mechanic player)
        {
            if (HoldingPlayer == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            base.UseTool(player);

            SuccesfulHits++;
            if (SuccesfulHits >= MaxSuccesfulHits)
            {
                if (!SuccesfulPlayers.Contains(HoldingPlayer.gameObject))
                {
                    SuccesfulPlayers.Add(HoldingPlayer.gameObject);
                }
                Debug.LogWarning("Succesful Fix TODO Hook up to Robo");
            }
        }
    }
}
