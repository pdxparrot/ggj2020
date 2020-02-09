using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.World;

using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    // TODO: rename MechanicBehavior and make an ActorComponent
    [RequireComponent(typeof(Interactables3D))]
    public sealed class Mechanic : MonoBehaviour
    {
        [SerializeField]
        private Player _owner;

        public Player Owner => _owner;

        [SerializeField]
        private BoneFollower _toolAttachment;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Tool _heldTool;

        public bool HasTool => _heldTool != null;

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
        [ReadOnly]
        private bool _canUseTool;

        public bool CanUseTool
        {
            get => GameManager.Instance.MechanicsCanInteract && HasTool && !IsUsingTool && _canUseTool;
            private set
            {
                _canUseTool = value;
                // TODO: notify the tool so it can whatever it needs to with its sprite thingy
            }
        }

        public bool IsUsingTool => HasTool && _heldTool.InUse;

        [Space(10)]

#region Effects
        [Header("Effects")]

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
#endregion

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

        public RepairPoint GetDamagedRepairPoint(RepairPoint.DamageType damageType)
        {
            var interactables = _interactables.GetInteractables<RepairPoint>();
            foreach(IInteractable interactable in interactables) {
                RepairPoint repairPoint = (RepairPoint)interactable;
                if(!repairPoint.IsRepaired && repairPoint.RepairPointDamageType == damageType) {
                    return repairPoint;
                }
            }
            return null;
        }

        public void HandleLadderInput()
        {
            if(IsUsingTool) {
                return;
            }

            if(IsOnLadder) {
                IsOnLadder = false;
            } else if(CanUseLadder) {
                Owner.GamePlayerBehavior.ClimbLadderEffectTrigger.Trigger();
                IsOnLadder = true;
            }
        }

        private bool UseTool()
        {
            if(!CanUseTool) {
                return false;
            }

            if(!_heldTool.UseTool()) {
                return false;
            }

            _useToolEffect.Trigger();
            return true;
        }

        private void PickupTool()
        {
            Tool tool = _interactables.GetRandomInteractable<Tool>();
            if(null == tool) {
                return;
            }

            if(!tool.SetHeld(this)) {
                return;
            }

            _heldTool = tool;
            _interactables.RemoveInteractable(_heldTool);

            _heldTool.transform.SetParent(_toolAttachment.transform);

            _pickupToolEffect.Trigger();
        }

        // returns true if we're using a tool
        public bool HandleToolInput()
        {
            if(IsOnLadder) {
                return false;
            }

            if(UseTool()) {
                return true;
            }

            PickupTool();
            return false;
        }

        public void TrackThumbStickAxis(Vector2 axis)
        {
            if(!IsUsingTool) {
                return;
            }

            _heldTool.TrackThumbStickAxis(axis);
        }

        public void UseEnded()
        {
            if(!IsUsingTool) {
                return;
            }

            _heldTool.EndUseTool();
            Owner.Behavior.SpineAnimationHelper.SetEmptyAnimation(1);
        }

        public void DropTool()
        {
            if(!HasTool || IsUsingTool) {
                return;
            }

            _heldTool.Drop();
            _interactables.AddInteractable(_heldTool);
            _heldTool = null;

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

            Owner.UIBubble.HideSprite();
        }

#region Events
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            if(!CanUseLadder && args.Interactable.gameObject.GetComponent<Ladder>() != null) {
                CanUseLadder = true;
            }

            if(!CanUseTool && args.Interactable.gameObject.GetComponent<RepairPoint>() != null) {
                CanUseTool = true;
            }
        }

        private void InteractableRemovedEventHandler(object sender, InteractableEventArgs args)
        {
            if(args.Interactable.gameObject.GetComponent<Ladder>() != null && !_interactables.HasInteractables<Ladder>()) {
                CanUseLadder = false;
            }

            if(args.Interactable.gameObject.GetComponent<RepairPoint>() != null && !_interactables.HasInteractables<RepairPoint>()) {
                CanUseTool = false;
            }
        }
#endregion
    }
}
