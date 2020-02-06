using UnityEngine;

using pdxpartyparrot.ggj2020.Players;

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
        private void Start()
        {
            ButtonHeld = false;
            DType = Actors.RepairPoint.DamageType.Loose;
        }

        // Update is called once per frame
        private void Update()
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
        public override void UseTool(Mechanic player)
        {
            if (closestPoint == null || HoldingPlayer.gameObject != player.gameObject)
                return;

            if (closestPoint.RepairPointDamageType != DType)
                return;

            base.UseTool(player);

            ButtonHeld = true;

            HoldingPlayer.WrenchEffect.gameObject.SetActive(true);
            HoldingPlayer.WrenchEffect.Trigger();
        }

        public override void EndUseTool()
        {
            HoldingPlayer.WrenchEffect.StopTrigger();
            HoldingPlayer.WrenchEffect.gameObject.SetActive(false);

            ButtonHeld = false;
            SuccessfulTurns = 0;
            LastTurnAxis = 1;

            base.EndUseTool();
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
                Debug.Log("Repair Done!");
            }
        }

        public override void SetAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.SetAttachment("Tool_Wrench", "Tool_Wrench");
            }
        }

        public override void RemoveAttachment()
        {
            Player pl = HoldingPlayer.GetComponent<Player>();
            if (pl != null)
            {
                pl.MechanicModel.RemoveAttachment("Tool_Wrench");
            }
        }
    }
}
