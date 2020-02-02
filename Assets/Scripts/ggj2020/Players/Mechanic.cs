using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.ggj2020.Tools;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    public class Mechanic : MonoBehaviour
    {
        [SerializeField]
        private Player _owner;

        public Player Owner => _owner;

        private Tool held_tool = null;
        private Tool collided_tool = null;

        [SerializeField]
        private EffectTrigger _pickupToolEffect;

        [SerializeField]
        private EffectTrigger _dropToolEffect;

        [SerializeField]
        private EffectTrigger _useToolEffect;

        bool HasTool()
        {
            return held_tool != null;
        }

        public void UseOrPickupTool()
        {
            if (HasTool() && GameManager.Instance.MechanicsCanInteract)
            {
                held_tool.UseTool(this);
                _useToolEffect.Trigger();
            } else if (held_tool == null && collided_tool != null)
            {
                held_tool = collided_tool;
                held_tool.SetHeld(this);
                _pickupToolEffect.Trigger();
            }
        }

        public void TrackThumbStickAxis(Vector2 Axis)
        {
            if (HasTool() && GameManager.Instance.MechanicsCanInteract)
            {
                held_tool.TrackThumbStickAxis(Axis);
            }
        }

        public void UseEnded()
        {
            if (HasTool())
            {
                held_tool.EndUseTool();
                Owner.Behavior.SpineAnimationHelper.SetEmptyAnimation(1);
            }
        }

        public void DropTool()
        {
            if (held_tool == null)
                return;

            held_tool.Drop();
            held_tool = null;

            _dropToolEffect.Trigger();
        }

        public void SetCollidedTool(Tool new_tool)
        {
            collided_tool = new_tool;
        }

        Tool GetHeldTool()
        {
            return held_tool;
        }

        void OnTriggerEnter(Collider collision)
        {
            collided_tool = collision.gameObject.GetComponent<Tool>();
        }
    }
}
