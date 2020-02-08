using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Tools;
using pdxpartyparrot.ggj2020.World;

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

        public void ClimbLadder(bool climb)
        {
            if(climb) {
                Owner.GamePlayerBehavior.ClimbLadderEffectTrigger.Trigger();
            }
            IsOnLadder = climb;
        }

        public void UseOrPickupTool()
        {
            // use the tool we're holding if we can
            if(HasTool) {
                if(!GameManager.Instance.MechanicsCanInteract) {
                    return;
                }

                if(_heldTool.UseTool()) {
                    _useToolEffect.Trigger();
                }
                return;
            }

            // if not, try to pick on eup
            _heldTool = _interactables.GetRandomInteractable<Tool>();
            if(null == _heldTool) {
                return;
            }
            _interactables.RemoveInteractable(_heldTool);

            if(_heldTool.SetHeld(this)) {
                _pickupToolEffect.Trigger();
            }
        }

        public void TrackThumbStickAxis(Vector2 axis)
        {
            if(HasTool) {
                _heldTool.TrackThumbStickAxis(axis);
            }
        }

        public void UseEnded()
        {
            if(_heldTool) {
                _heldTool.EndUseTool();
                Owner.Behavior.SpineAnimationHelper.SetEmptyAnimation(1);
            }
        }

        public void DropTool()
        {
            if(_heldTool == null) {
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
        }

#region Events
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            CanUseLadder = args.Interactable.gameObject.GetComponent<Ladder>() != null;
        }

        private void InteractableRemovedEventHandler(object sender, InteractableEventArgs args)
        {
            if(args.Interactable.gameObject.GetComponent<Ladder>() != null) {
                CanUseLadder = false;
            }
        }
#endregion
    }
}
