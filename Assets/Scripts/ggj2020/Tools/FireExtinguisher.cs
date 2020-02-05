using UnityEngine;

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
            DType = Actors.RepairPoint.DamageType.Fire;
        }

        // Update is called once per frame
        void Update()
        {
            if (HoldingPlayer == null)
                return;

            // -- TODO update this once functions have been moved
            closestPoint = FindClosestRepairPoint(FindRepairPoints(), DType);
            if(!GameManager.Instance.MechanicsCanInteract || closestPoint == null)
            {
                HoldingPlayer.Owner.UIBubble.HideSprite();
                return;
            }

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();
            if (ButtonHeld)
            {
                // -- make sure you don't repair multiple points
                if (closestPoint != oldClosestPoint)
                {
                    oldClosestPoint = closestPoint;
                    TimeAtStartOfHold = Time.realtimeSinceStartup;
                }

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

            if (closestPoint.RepairPointDamageType != DType)
                return;

            base.UseTool(player);

            ButtonHeld = true;
            TimeAtStartOfHold = Time.realtimeSinceStartup;
        }

        override public void SetAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.SetAttachment("Tool_FireExtinguisher", "Tool_FireExtinguisher");
            }
        }

        override public void RemoveAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.RemoveAttachment("Tool_FireExtinguisher");
            }
        }
    }
}
