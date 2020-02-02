using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using pdxpartyparrot.ggj2020.Tools;
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
        void Start()
        {
            TimeSinceLastWindow = 0;
            CurrentTime = 0;
            ButtonHeld = false;
        }

        // Update is called once per frame
        void Update()
        {
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
            if (HoldingPlayer.gameObject != player.gameObject)
                return;

            float delta = CurrentTime - TimeSinceLastWindow;
            if (delta >= TimeBetweenPresses && (delta - TimeBetweenPresses) < TimeToAllowSuccesfulPress)
            {
                SuccesfulHits++;
                TimeSinceLastWindow = CurrentTime;
                //print("Hammer hit");
            }
            //else
            //{
            //    print("Hammer failed hit");
            //}
           
            if (SuccesfulHits >= MaxSuccesfulHits)
            {
                print("Succesful fix TODO connect to robot for fix");
            }

        }
    }
}
