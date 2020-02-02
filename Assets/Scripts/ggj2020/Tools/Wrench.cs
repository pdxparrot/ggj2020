using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.UI;

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
            if (HoldingPlayer == null)
                return;

            // -- TODO update this once functions have been moved
            UIBubble bubble = HoldingPlayer.GetComponentInChildren<UIBubble>();
            closestPoint = FindClosestRepairPoint(FindRepairPoints());
            if (closestPoint == null) {
                bubble.HideSprite();
                return;
            }

            if (LastTurnAxis != 1)
            {
                bubble.SetThumbRight();
            }
            else if (LastTurnAxis != -1)
            {
                bubble.SetThumbLeft();
            }
        }
        override public void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.GetDamageType() != Actors.RepairPoint.DamageType.Loose)
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
            if (closestPoint == null && !ButtonHeld)
                return;


            if ((Axis.x >= 1 || Axis.y >= 1) && LastTurnAxis != 1)
            {
                LastTurnAxis = 1;
            } else if ((Axis.x <= -1 || Axis.y <= -1) && LastTurnAxis != -1)
            {
                LastTurnAxis = -1;
                SuccessfulTurns++;
            }

            if (SuccessfulTurns >= MaxSuccesfulTurns && !closestPoint.IsRepaired)
            {
                closestPoint.Repair();
                print("Repair Done!");
            }
        }
    }
}
