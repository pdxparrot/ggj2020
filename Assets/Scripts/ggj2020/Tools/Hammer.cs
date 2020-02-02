using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Actors;

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
        private RepairPoint oldClosestPoint = null;
        
        // Start is called before the first frame update
        void Start()
        {
            TimeSinceLastWindow = 0;
            CurrentTime = 0;
            ButtonHeld = false;
        }

        // Update is called once per frame
        void Update()
        {
            // -- TODO update this once functions have been moved
            closestPoint = FindClosestRepairPoint(FindRepairPoints());
            if (closestPoint == null)
                return;

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

            if (PrintWindowToConsole)
            {
                if (delta >= TimeBetweenPresses && (delta - TimeBetweenPresses) < TimeToAllowSuccesfulPress)
                {
                    print("Hammer window is open");
                }
                else
                {
                    print("Hammer window is closed");
                }
            }
            
        }

        override public void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.GetDamageType() != Actors.RepairPoint.DamageType.Damaged)
                return;

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
                print("Repair Done!");

            }

        }
    }
}
