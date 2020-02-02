using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Tools;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Wrench : Tool
    {
        public int MaxSuccesfulTurns = 5;
        private int LastTurnAxis = 1;
        private bool ButtonHeld = false;
        private Vector2 OldCurrentAxis;
        private int SuccessfulTurns = 0;
        // Start is called before the first frame update
        void Start()
        {
            ButtonHeld = false;
        }

        // Update is called once per frame
        void Update()
        {

        }
        override public void UseTool(Mechanic player)
        {
            if (HoldingPlayer.gameObject != player.gameObject)
                return;

            ButtonHeld = true;
        }

        public override void EndUseTool()
        {
            ButtonHeld = false;
            SuccessfulTurns = 0;
            LastTurnAxis = 1;
        }

        public override void TrackThumbStickAxis(Vector2 Axis)
        {
            if ((Axis.x >= 1 || Axis.y >= 1) && LastTurnAxis != 1)
            {
                LastTurnAxis = 1;
            } else if ((Axis.x <= -1 || Axis.y <= -1) && LastTurnAxis != -1)
            {
                LastTurnAxis = -1;
                SuccessfulTurns++;
                print("Turn was made");
            }

            if (SuccessfulTurns >= MaxSuccesfulTurns)
            {
                print("Succesful Fix TODO hook up to the robo");
            }
        }
    }
}
