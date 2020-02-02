using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.Players;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class BatteryStation : Tool
    {
        public int MaxSuccesfulHits = 25;
        private int SuccesfulHits = 0;

        List<GameObject> SuccesfulPlayers = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
           
        }

        override public void SetHeld(Mechanic player)
        {
            HoldingPlayer = player;
        }

        override public void Drop()
        {

        }

        override public void PlayerExitTrigger()
        {
            if (HoldingPlayer == null)
                return;

            HoldingPlayer.DropTool();
            HoldingPlayer = null;
            SuccesfulHits = 0;
        }

        override public void UseTool(Mechanic player)
        {
            if (HoldingPlayer == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            SuccesfulHits++;
            if (SuccesfulHits >= MaxSuccesfulHits)
            {
                if (!SuccesfulPlayers.Contains(HoldingPlayer.gameObject))
                {
                    SuccesfulPlayers.Add(HoldingPlayer.gameObject);
                }
                print("Succesful Fix TODO Hook up to Robo");
            }
        }
    }
}
