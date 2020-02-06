﻿using UnityEngine;

using pdxpartyparrot.ggj2020.Players;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class Hammer : Tool
    {
        public float TimeBetweenPresses = 1.5f;
        public float TimeToAllowSuccesfulPress = 1;
        public int MaxSuccesfulHits = 0;
        public bool PrintWindowToConsole = false;
        private float TimeSinceLastWindow = 0;
        private float CurrentTime = 0;
        private bool ButtonHeld = false;
        private int SuccesfulHits = 0;
        
        // Start is called before the first frame update
        private void Start()
        {
            TimeSinceLastWindow = 0;
            CurrentTime = 0;
            ButtonHeld = false;
            DType = Actors.RepairPoint.DamageType.Damaged;
        }

        // Update is called once per frame
        private void Update()
        {
            if (HoldingPlayer == null)
                return;

            // -- TODO update this once functions have been moved
            closestPoint = FindClosestRepairPoint(FindRepairPoints(), DType);
            if (!GameManager.Instance.MechanicsCanInteract || closestPoint == null)
            {
                HoldingPlayer.Owner.UIBubble.HideSprite();
                return;
            }

            // -- make sure you don't repair multiple points
            if (closestPoint != oldClosestPoint)
            {
                oldClosestPoint = closestPoint;
                SuccesfulHits = 0;
            }

            CurrentTime = Time.realtimeSinceStartup;
            float delta = CurrentTime - TimeSinceLastWindow;
            if ((delta - TimeBetweenPresses) >= TimeToAllowSuccesfulPress)
            {
                TimeSinceLastWindow = CurrentTime;
            }

            
            if (delta >= TimeBetweenPresses && (delta - TimeBetweenPresses) < TimeToAllowSuccesfulPress)
            {
                HoldingPlayer.Owner.UIBubble.SetPressedSprite();
                if (PrintWindowToConsole)
                    Debug.Log("Hammer window is open");
            }
            else
            {
                HoldingPlayer.Owner.UIBubble.SetUnpressedSprite();
                if (PrintWindowToConsole)
                    Debug.Log("Hammer window is closed");
            }
        }

        public override void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.RepairPointDamageType != DType)
                return;

            base.UseTool(player);

            float delta = CurrentTime - TimeSinceLastWindow;
            if (delta >= TimeBetweenPresses && (delta - TimeBetweenPresses) < TimeToAllowSuccesfulPress)
            {
                SuccesfulHits++;
                TimeSinceLastWindow = CurrentTime;
            }
           
            if (SuccesfulHits >= MaxSuccesfulHits && !closestPoint.IsRepaired)
            {
                closestPoint.Repair();
                SuccesfulHits = 0;
                Debug.Log("Repair Done!");

            }

        }

        public override void SetAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.SetAttachment("Tool_Hammer", "Tool_Hammer");
            }
        }

        public override void RemoveAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.RemoveAttachment("Tool_Hammer");
            }
        }
    }
}
