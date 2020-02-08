using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Tools
{
    public sealed class Hammer : Tool
    {
        // TODO: move to data
        [SerializeField]
        private float _timeToAllowSuccesfulPress = 1;

        [SerializeField]
        private int _maxSuccesfulHits = 5;

        [SerializeField]
        [ReadOnly]
        private int _succesfulHits;

        public override bool UseTool()
        {
            if(!base.UseTool()) {
                return false;
            }

            if(HoldingPlayer.HammerEffect.IsRunning) {
                return false;
            }

            // find a point to repair
            if(null == RepairPoint) {
                RepairPoint = HoldingPlayer.GetDamagedRepairPoint(DamageType);
                if(RepairPoint == null) {
                    return false;
                }
            }

            HoldingPlayer.Owner.UIBubble.SetPressedSprite();

            HoldingPlayer.HammerEffect.gameObject.SetActive(true);
            HoldingPlayer.HammerEffect.Trigger(() => {
                _succesfulHits++;
                if(_succesfulHits >= _maxSuccesfulHits && !RepairPoint.IsRepaired) {
                    RepairPoint.Repair();
                    RepairPoint = null;

                    _succesfulHits = 0;
                    Debug.Log("Repair Done!");
                    return;
                }

                HoldingPlayer.Owner.UIBubble.SetUnpressedSprite();
            });

            return true;
        }

        public override void EndUseTool()
        {
            HoldingPlayer.HammerEffect.StopTrigger();
            HoldingPlayer.HammerEffect.gameObject.SetActive(false);

            base.EndUseTool();
        }

        public override void SetAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.SetAttachment("Tool_Hammer", "Tool_Hammer");
        }

        public override void RemoveAttachment()
        {
            HoldingPlayer.Owner.MechanicModel.RemoveAttachment("Tool_Hammer");
        }
    }
}
