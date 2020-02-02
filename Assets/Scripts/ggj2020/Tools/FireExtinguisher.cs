using System.Collections;
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
            if (ButtonHeld)
            {
                CurrentTime = Time.realtimeSinceStartup;
                float delta = CurrentTime - TimeAtStartOfHold;
                if (delta >= HoldTime)
                {
                    print("Succesful Fix TODO hook up with robot");
                }
            }
           
        }

        override public void UseTool(Mechanic player)
        {
            if (HoldingPlayer.gameObject != player.gameObject)
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
