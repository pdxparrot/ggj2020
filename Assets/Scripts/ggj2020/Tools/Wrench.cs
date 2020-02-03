﻿using System.Collections;
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
            DType = Actors.RepairPoint.DamageType.Loose;

        }

        // Update is called once per frame
        void Update()
        {
            if (HoldingPlayer == null)
                return;

            // -- TODO update this once functions have been moved
            closestPoint = FindClosestRepairPoint(FindRepairPoints(), DType);
            if (!GameManager.Instance.MechanicsCanInteract || closestPoint == null) {
                HoldingPlayer.Owner.UIBubble.HideSprite();
                return;
            }

            // -- make sure you don't repair multiple points
            if (closestPoint != oldClosestPoint)
            {
                oldClosestPoint = closestPoint;
                SuccessfulTurns = 0;
            }

            if (LastTurnAxis != 1)
            {
                HoldingPlayer.Owner.UIBubble.SetThumbRight();
            }
            else if (LastTurnAxis != -1)
            {
                HoldingPlayer.Owner.UIBubble.SetThumbLeft();
            }
        }
        override public void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.RepairPointDamageType != DType)
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
            if (closestPoint == null || !ButtonHeld)
                return;


            if ((Axis.x >= .5f || Axis.y >= .5f) && LastTurnAxis != 1)
            {
                LastTurnAxis = 1;
            } else if ((Axis.x <= -.5f || Axis.y <= -.5f) && LastTurnAxis != -1)
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

        override public void SetAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.SetAttachment("Tool_Wrench", "Tool_Wrench");
            }
        }

        override public void RemoveAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.RemoveAttachment("Tool_Wrench");
            }
        }
    }
}
