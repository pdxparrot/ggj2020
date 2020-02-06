using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.World;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    [RequireComponent(typeof(Interactables3D))]
    public class Mechanic : MonoBehaviour
    {
        [SerializeField]
        private Player _owner;

        public Player Owner => _owner;

        [SerializeField]
        [ReadOnly]
        private Tool held_tool = null;

        public bool HasTool => held_tool != null;

        [SerializeField]
        [ReadOnly]
        private Tool collided_tool = null;

        [SerializeField]
        [ReadOnly]
        private bool _canUseLadder;

        public bool CanUseLadder
        {
            get => _canUseLadder;
            private set
            {
                _canUseLadder = value;
                IsOnLadder = IsOnLadder && _canUseLadder;
            }
        }

        [SerializeField]
        [ReadOnly]
        private bool _isOnLadder;

        public bool IsOnLadder
        {
            get => _isOnLadder;
            private set
            {
                _isOnLadder = value;
                Owner.Movement.IsKinematic = value;
            }
        }

        [SerializeField]
        private EffectTrigger _pickupToolEffect;

        [SerializeField]
        private EffectTrigger _dropToolEffect;

        [SerializeField]
        private EffectTrigger _useToolEffect;

        [SerializeField]
        private EffectTrigger _fireExtinguisherEffect;

        public EffectTrigger FireExtinguisherEffect => _fireExtinguisherEffect;

        [SerializeField]
        private EffectTrigger _hammerEffect;

        public EffectTrigger HammerEffect => _hammerEffect;

        [SerializeField]
        private EffectTrigger _wrenchEffect;

        public EffectTrigger WrenchEffect => _wrenchEffect;

        private Interactables _interactables;

#region Unity Lifecycle
        private void Awake()
        {
            _interactables = GetComponent<Interactables>();
            _interactables.InteractableAddedEvent += InteractableAddedEventHandler;
            _interactables.InteractableRemovedEvent += InteractableRemovedEventHandler;

            DisableToolEffects();
        }
#endregion

        public void ClimbLadder(bool climb)
        {
            if(climb) {
                Owner.GamePlayerBehavior.ClimbLadderEffectTrigger.Trigger();
            }
            IsOnLadder = climb;
        }

        public void UseOrPickupTool()
        {
            if (HasTool && GameManager.Instance.MechanicsCanInteract) {
                held_tool.UseTool(this);
                _useToolEffect.Trigger();
            } else if (held_tool == null && collided_tool != null) {
                held_tool = collided_tool;
                held_tool.SetHeld(this);
                _pickupToolEffect.Trigger();
            }
        }

        public void TrackThumbStickAxis(Vector2 Axis)
        {
            if (HasTool && GameManager.Instance.MechanicsCanInteract)
            {
                held_tool.TrackThumbStickAxis(Axis);
            }
        }

        public void UseEnded()
        {
            if (HasTool)
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

            DisableToolEffects();
        }

        private void DisableToolEffects()
        {
            _fireExtinguisherEffect.StopTrigger();
            _fireExtinguisherEffect.gameObject.SetActive(false);

            _hammerEffect.StopTrigger();
            _hammerEffect.gameObject.SetActive(false);

            _wrenchEffect.StopTrigger();
            _wrenchEffect.gameObject.SetActive(false);
        }

#region Events
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            // this might make sense? I dunno...
            Tool tool = args.Interactable.gameObject.GetComponent<Tool>();
            if(tool != null) {
                collided_tool = tool;
            }

            CanUseLadder = args.Interactable.gameObject.GetComponent<Ladder>() != null;
        }

        private void InteractableRemovedEventHandler(object sender, InteractableEventArgs args)
        {
            Tool tool = args.Interactable.gameObject.GetComponent<Tool>();
            if(null != tool) {
                tool.PlayerExitTrigger();

                collided_tool = _interactables.GetRandomInteractable<Tool>();
            }

            if(args.Interactable.gameObject.GetComponent<Ladder>() != null) {
                CanUseLadder = false;
            }
        }
#endregion
    }
}
