﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.Players;

namespace pdxpartyparrot.ggj2020.Tools
{
    public class FireExtinguisher : Tool
    {
        public float HoldTime = 1;
        private float CurrentTime = 0;
        private float TimeAtStartOfHold = 0;
        private bool ButtonHeld = false;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            // -- TODO update this once functions have been moved
            closestPoint = FindClosestRepairPoint(FindRepairPoints());
            if(closestPoint == null)
                return;

            if (ButtonHeld)
            {
                CurrentTime = Time.realtimeSinceStartup;
                float delta = CurrentTime - TimeAtStartOfHold;
                if (delta >= HoldTime && !closestPoint.IsRepaired)
                {
                    closestPoint.Repair();
                    print("Repair Done!");
                }
            }
            else
            {
                CurrentTime = Time.realtimeSinceStartup;
                TimeAtStartOfHold = CurrentTime;
            }
        }

        override public void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.GetDamageType() != Actors.RepairPoint.DamageType.Fire)
                return;

            ButtonHeld = true;
            TimeAtStartOfHold = Time.realtimeSinceStartup;
        }

        public override void EndUseTool()
        {
            ButtonHeld = false;
        }
    }
}
