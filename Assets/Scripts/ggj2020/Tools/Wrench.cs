using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class Wrench : Tool
    {
        [SerializeField]
        private int _maxSuccesfulTurns = 5;

        [SerializeField]
        [ReadOnly]
        private int _lastTurnAxis = 1;

        [SerializeField]
        [ReadOnly]
        private int _successfulTurns;

#region Unity Lifecycle
        private void Update()
        {
            if(!InUse) {
                return;
            }

            if(_lastTurnAxis != 1) {
                HoldingPlayer.Owner.UIBubble.SetThumbRight();
            } else if (_lastTurnAxis != -1) {
                HoldingPlayer.Owner.UIBubble.SetThumbLeft();
            }
        }
#endregion

        public override bool UseTool()
        {
            if(!base.UseTool()) {
                return false;
            }

            // find a point to repair
            RepairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
            if(RepairPoint == null) {
                base.EndUseTool();
                return false;
            }

            _lastTurnAxis = 1;

            HoldingPlayer.WrenchEffect.gameObject.SetActive(true);
            HoldingPlayer.WrenchEffect.Trigger();

            return true;
        }

        public override void EndUseTool()
        {
            HoldingPlayer.WrenchEffect.StopTrigger();
            HoldingPlayer.WrenchEffect.gameObject.SetActive(false);

            _successfulTurns = 0;

            base.EndUseTool();
        }

        public override void TrackThumbStickAxis(Vector2 axis)
        {
            if(!InUse || !GameManager.Instance.MechanicsCanInteract)
                return;

            if((axis.x >= .5f || axis.y >= .5f) && _lastTurnAxis != 1) {
                _lastTurnAxis = 1;
            } else if ((axis.x <= -.5f || axis.y <= -.5f) && _lastTurnAxis != -1) {
                _lastTurnAxis = -1;
                _successfulTurns++;
            }

            if(_successfulTurns >= _maxSuccesfulTurns && !RepairPoint.IsRepaired) {
                RepairPoint.Repair();
                Debug.Log("Repair Done!");
            }
        }

        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_Wrench", "Tool_Wrench");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_Wrench");
        }
    }
}
